# Render 배포 가이드

## 사전 준비

1. **Render 계정**: https://render.com 에서 가입
2. **GitHub 저장소**: 코드를 GitHub에 푸시
3. **PostgreSQL 연결 정보**: 기존 Render PostgreSQL 인스턴스 정보

## 배포 단계

### 1. GitHub 저장소에 코드 푸시

```bash
cd C:\Users\28400\Desktop\ChurchWeb
git init  # 아직 git 저장소가 아니라면
git add .
git commit -m "Render 배포 준비 완료"
git remote add origin <your-github-repo-url>
git push -u origin main
```

### 2. Render 대시보드에서 새 Web Service 생성

1. Render 대시보드 접속: https://dashboard.render.com
2. **New +** 버튼 클릭 → **Web Service** 선택
3. GitHub 저장소 연결
4. 다음 설정 입력:

#### 기본 설정
- **Name**: `churchweb` (또는 원하는 이름)
- **Region**: `Singapore`
- **Runtime**: `Docker`
- **Dockerfile Path**: `./Dockerfile`
- **Plan**: `Starter` (유료) 또는 `Free` (무료, 디스크 미지원)

### 3. 환경 변수 설정

**Environment Variables** 섹션에서 다음 변수 추가:

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__Default=<Render PostgreSQL 연결 문자열>
UPLOADS_PATH=/var/data/uploads
DP_KEYS_PATH=/var/data/keys
YouTube__ApiKey=<YouTube API 키> (선택사항)
KakaoMap__ApiKey=<Kakao Map API 키> (선택사항)
```

**PostgreSQL 연결 문자열 형식:**
```
Host=<your-host>;Port=5432;Database=<db-name>;Username=<username>;Password=<password>;SSL Mode=Require;Trust Server Certificate=true
```

### 4. Persistent Disk 추가 (중요!)

**Persistent Disks** 섹션에서:
1. **Add Disk** 클릭
2. 다음 정보 입력:
   - **Name**: `churchweb-data`
   - **Mount Path**: `/var/data`
   - **Size**: `1 GB` (필요에 따라 조정)
3. **Save** 클릭

> **중요**: Disk를 추가하면 **단일 인스턴스만** 실행 가능합니다.

### 5. PostgreSQL 데이터베이스 연결

#### 옵션 A: 기존 Render PostgreSQL 사용
이미 PostgreSQL 인스턴스가 있다면:
1. 연결 문자열을 환경 변수 `ConnectionStrings__Default`에 입력
2. 기존 데이터베이스 사용

#### 옵션 B: 새 PostgreSQL 인스턴스 생성
1. Render 대시보드 → **New +** → **PostgreSQL**
2. 다음 설정:
   - **Name**: `churchweb-db`
   - **Database**: `churchweb`
   - **Region**: `Singapore`
   - **Plan**: `Starter` (유료) 또는 `Free` (무료)
3. 생성 후 **Internal Database URL** 복사
4. Web Service의 환경 변수 `ConnectionStrings__Default`에 붙여넣기

### 6. 배포 시작

1. 모든 설정 완료 후 **Create Web Service** 클릭
2. 자동으로 Docker 이미지 빌드 및 배포 시작
3. **Logs** 탭에서 배포 진행 상황 확인

### 7. 배포 완료 확인

배포가 완료되면:
1. **Logs** 탭에서 "Application started" 메시지 확인
2. Render가 제공하는 URL 접속 (예: `https://churchweb.onrender.com`)
3. 홈페이지 로드 확인
4. 관리자 로그인 테스트:
   - URL: `https://your-app.onrender.com/Account/Login`
   - Username: `admin`
   - Password: `Admin@2026!`

### 8. 기능 테스트

배포 후 다음 기능들을 테스트하세요:

- [ ] 홈페이지 로드
- [ ] 관리자 로그인
- [ ] Blazor 폼 동작 (WebSocket 연결)
- [ ] 이미지 업로드 (갤러리, 공지사항)
- [ ] 업로드한 이미지 표시 확인
- [ ] 재배포 후:
  - [ ] 관리자 로그인 유지 (DataProtection 키 영속화)
  - [ ] 업로드한 파일 유지 (Persistent Disk)
  - [ ] DB 데이터 유지

## 로컬 Docker 테스트 (선택사항)

Render에 배포하기 전 로컬에서 테스트:

```bash
# 1. Docker 이미지 빌드
docker build -t churchweb:latest .

# 2. 컨테이너 실행 (환경 변수 설정)
docker run -d \
  -p 8080:8080 \
  -e ConnectionStrings__Default="Host=<your-host>;Port=5432;..." \
  -e UPLOADS_PATH=/tmp/uploads \
  -e DP_KEYS_PATH=/tmp/keys \
  -e ASPNETCORE_ENVIRONMENT=Production \
  --name churchweb \
  churchweb:latest

# 3. 로그 확인
docker logs -f churchweb

# 4. 브라우저에서 http://localhost:8080 접속

# 5. 테스트 완료 후 정리
docker stop churchweb
docker rm churchweb
```

## 문제 해결

### 1. 빌드 실패
- **Logs** 탭에서 에러 메시지 확인
- .NET 버전 확인 (.NET 10.0)
- 프로젝트 파일 경로 확인

### 2. 데이터베이스 연결 실패
- 환경 변수 `ConnectionStrings__Default` 확인
- PostgreSQL 인스턴스 실행 상태 확인
- 연결 문자열 형식 확인 (Npgsql 형식)

### 3. 파일 업로드 실패
- Persistent Disk 마운트 확인 (`/var/data`)
- 환경 변수 `UPLOADS_PATH` 확인
- 디스크 용량 확인

### 4. 관리자 로그인 유지 안 됨
- Persistent Disk 마운트 확인
- 환경 변수 `DP_KEYS_PATH` 확인
- `/var/data/keys` 디렉터리 존재 여부 확인

### 5. Blazor WebSocket 연결 실패
- ForwardedHeaders 설정 확인 (이미 추가됨)
- Render는 WebSocket을 자동 지원

## 자동 배포 (CD)

GitHub에 코드를 푸시하면 자동으로 Render에 배포됩니다:

```bash
git add .
git commit -m "업데이트 메시지"
git push origin main
```

Render가 자동으로:
1. 새 커밋 감지
2. Docker 이미지 재빌드
3. 새 버전 배포
4. 무중단 배포 (Blue-Green)

## 비용

### Starter Plan (권장)
- **Web Service**: $7/월
- **PostgreSQL**: $7/월
- **Persistent Disk**: 1GB당 $0.25/월
- **총 예상 비용**: 약 $14.25/월

### Free Plan (제한적)
- **Web Service**: 무료 (750시간/월)
- **PostgreSQL**: 무료 (90일 후 삭제, 1GB)
- **Persistent Disk**: 미지원 (업로드 파일 유지 불가)

## 참고 링크

- Render 문서: https://render.com/docs
- Docker 배포: https://render.com/docs/docker
- Persistent Disks: https://render.com/docs/disks
- PostgreSQL: https://render.com/docs/databases
