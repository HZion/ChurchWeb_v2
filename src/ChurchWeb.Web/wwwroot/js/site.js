// ===== 스크롤 시 헤더 배경 전환 =====
const header = document.getElementById('siteHeader');
if (header) {
    window.addEventListener('scroll', () => {
        header.classList.toggle('scrolled', window.scrollY > 60);
    });
}

// ===== 메가 메뉴 호버 시 헤더 배경 활성화 =====
const navcenter = document.querySelector('.navcenter');
if (navcenter && header) {
    navcenter.addEventListener('mouseenter', () => header.classList.add('menu-open'));
    navcenter.addEventListener('mouseleave', () => header.classList.remove('menu-open'));
}

// ===== 라이트/다크 토글 =====
const site = document.getElementById('site');
const themeToggle = document.getElementById('themeToggle');

if (site && themeToggle) {
    // 로컬 스토리지에서 테마 불러오기
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
        site.classList.add('dark');
        themeToggle.textContent = '☀';
    }

    // 토글 버튼 클릭 이벤트
    themeToggle.addEventListener('click', () => {
        const isDark = site.classList.toggle('dark');
        themeToggle.textContent = isDark ? '☀' : '☾';

        // 선택값 로컬 스토리지에 저장
        localStorage.setItem('theme', isDark ? 'dark' : 'light');
    });
}

// ===== 히어로 슬라이더 =====
const heroSlides = document.querySelectorAll('.hero .slide');
const heroCur = document.getElementById('heroCur');
const heroFill = document.getElementById('heroFill');
const heroNext = document.getElementById('heroNext');
const heroPrev = document.getElementById('heroPrev');

if (heroSlides.length > 0 && heroCur && heroFill && heroNext && heroPrev) {
    let currentIndex = 0;
    let slideTimer = null;
    const SLIDE_DURATION = 5000; // 5초

    function goToSlide(newIndex) {
        // 현재 슬라이드 비활성화
        heroSlides[currentIndex].classList.remove('active');

        // 새 인덱스 계산 (순환)
        currentIndex = (newIndex + heroSlides.length) % heroSlides.length;

        // 새 슬라이드 활성화
        heroSlides[currentIndex].classList.add('active');

        // 카운터 업데이트
        heroCur.textContent = String(currentIndex + 1).padStart(2, '0');

        // 진행 바 애니메이션 재시작
        heroFill.classList.remove('run');
        void heroFill.offsetWidth; // reflow 강제
        heroFill.classList.add('run');
    }

    function startSlider() {
        stopSlider();
        // 진행 바 애니메이션 시작
        heroFill.classList.remove('run');
        void heroFill.offsetWidth;
        heroFill.classList.add('run');

        // 자동 전환 타이머 시작
        slideTimer = setInterval(() => {
            goToSlide(currentIndex + 1);
        }, SLIDE_DURATION);
    }

    function stopSlider() {
        if (slideTimer) {
            clearInterval(slideTimer);
            slideTimer = null;
        }
    }

    // 다음 버튼
    heroNext.addEventListener('click', () => {
        goToSlide(currentIndex + 1);
        startSlider();
    });

    // 이전 버튼
    heroPrev.addEventListener('click', () => {
        goToSlide(currentIndex - 1);
        startSlider();
    });

    // 슬라이더 시작
    startSlider();
}

// ===== 모바일 햄버거 메뉴 =====
const hambBtn = document.querySelector('.hamb');
const nav = document.querySelector('.nav');

if (hambBtn && nav) {
    hambBtn.addEventListener('click', () => {
        nav.classList.toggle('active');

        // 모바일 메뉴 활성화 시 스타일 추가
        if (nav.classList.contains('active')) {
            nav.style.display = 'flex';
            nav.style.position = 'fixed';
            nav.style.top = 'var(--header-h)';
            nav.style.left = '0';
            nav.style.right = '0';
            nav.style.flexDirection = 'column';
            nav.style.background = 'var(--header-bg)';
            nav.style.borderBottom = '1px solid var(--line)';
            nav.style.padding = '20px 24px';
            nav.style.gap = '16px';
            nav.style.backdropFilter = 'blur(8px)';
            nav.style.boxShadow = 'var(--shadow)';
        } else {
            nav.style.display = '';
            nav.style.position = '';
            nav.style.top = '';
            nav.style.left = '';
            nav.style.right = '';
            nav.style.flexDirection = '';
            nav.style.background = '';
            nav.style.borderBottom = '';
            nav.style.padding = '';
            nav.style.gap = '';
            nav.style.backdropFilter = '';
            nav.style.boxShadow = '';
        }
    });

    // 메뉴 링크 클릭 시 메뉴 닫기
    const navLinks = nav.querySelectorAll('a');
    navLinks.forEach(link => {
        link.addEventListener('click', () => {
            if (window.innerWidth <= 860) {
                nav.classList.remove('active');
                nav.style.display = '';
                nav.style.position = '';
                nav.style.top = '';
                nav.style.left = '';
                nav.style.right = '';
                nav.style.flexDirection = '';
                nav.style.background = '';
                nav.style.borderBottom = '';
                nav.style.padding = '';
                nav.style.gap = '';
                nav.style.backdropFilter = '';
                nav.style.boxShadow = '';
            }
        });
    });

    // 윈도우 리사이즈 시 메뉴 초기화
    window.addEventListener('resize', () => {
        if (window.innerWidth > 860) {
            nav.classList.remove('active');
            nav.style.display = '';
            nav.style.position = '';
            nav.style.top = '';
            nav.style.left = '';
            nav.style.right = '';
            nav.style.flexDirection = '';
            nav.style.background = '';
            nav.style.borderBottom = '';
            nav.style.padding = '';
            nav.style.gap = '';
            nav.style.backdropFilter = '';
            nav.style.boxShadow = '';
        }
    });
}

// ===== 스크롤 시 섹션 애니메이션 (Intersection Observer) =====
const observerOptions = {
    root: null,
    rootMargin: '0px',
    threshold: 0.1 // 섹션의 10%가 보이면 트리거
};

const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.classList.add('visible');
            // 한 번 표시되면 다시 관찰하지 않음 (선택사항)
            // observer.unobserve(entry.target);
        }
    });
}, observerOptions);

// 모든 섹션 관찰 시작
document.addEventListener('DOMContentLoaded', () => {
    const sections = document.querySelectorAll('.section, .vision, .locate');
    sections.forEach(section => {
        observer.observe(section);
    });
});

// ===== Base64 파일 다운로드 헬퍼 함수 =====
window.downloadBase64File = function (base64Data, fileName) {
    const link = document.createElement('a');
    link.href = base64Data;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
