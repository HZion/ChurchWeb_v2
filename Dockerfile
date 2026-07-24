# 멀티스테이지 빌드: SDK로 빌드 → 런타임으로 실행

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 프로젝트 파일 복사 및 복원
COPY ["src/ChurchWeb.Web/ChurchWeb.Web.csproj", "ChurchWeb.Web/"]
COPY ["src/ChurchWeb.Application/ChurchWeb.Application.csproj", "ChurchWeb.Application/"]
COPY ["src/ChurchWeb.Core/ChurchWeb.Core.csproj", "ChurchWeb.Core/"]
COPY ["src/ChurchWeb.Infrastructure/ChurchWeb.Infrastructure.csproj", "ChurchWeb.Infrastructure/"]

RUN dotnet restore "ChurchWeb.Web/ChurchWeb.Web.csproj"

# 전체 소스 복사 및 빌드
COPY src/ .
WORKDIR "/src/ChurchWeb.Web"
RUN dotnet publish "ChurchWeb.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Render 퍼시스턴트 디스크용 디렉터리 생성
RUN mkdir -p /var/data/uploads /var/data/keys && chmod -R 755 /var/data

COPY --from=build /app/publish .

# Render는 PORT 환경변수를 주입 (기본값: 8080)
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

# PORT 환경변수가 있으면 사용하도록 entrypoint script 사용
ENTRYPOINT ["sh", "-c", "dotnet ChurchWeb.Web.dll --urls http://+:${PORT:-8080}"]
