// 갤러리 라이트박스 기능
// 사진 클릭 → 전체화면 확대 + 좌우 이동 + ESC/화살표 키

let currentIndex = 0;
let photosData = [];

function initLightbox(photos) {
    photosData = photos;
    const photoItems = document.querySelectorAll('.photo-item');
    const lightbox = document.getElementById('lightbox');
    const lightboxImg = lightbox.querySelector('.lightbox-img');
    const lightboxClose = lightbox.querySelector('.lightbox-close');
    const lightboxPrev = lightbox.querySelector('.lightbox-prev');
    const lightboxNext = lightbox.querySelector('.lightbox-next');
    const lightboxCurrent = lightbox.querySelector('.lightbox-counter .current');
    const lightboxOverlay = lightbox.querySelector('.lightbox-overlay');

    // 사진 클릭 → 라이트박스 열기
    photoItems.forEach(item => {
        item.addEventListener('click', function() {
            currentIndex = parseInt(this.getAttribute('data-index'));
            openLightbox();
        });
    });

    // 라이트박스 열기
    function openLightbox() {
        showPhoto(currentIndex);
        lightbox.style.display = 'flex';
        document.body.style.overflow = 'hidden'; // 스크롤 방지
    }

    // 라이트박스 닫기
    function closeLightbox() {
        lightbox.style.display = 'none';
        document.body.style.overflow = ''; // 스크롤 복원
    }

    // 사진 표시
    function showPhoto(index) {
        if (index < 0) index = photosData.length - 1;
        if (index >= photosData.length) index = 0;
        currentIndex = index;

        // <img> 태그를 동적으로 생성
        lightboxImg.innerHTML = `<img src="${photosData[index]}" alt="Photo ${index + 1}" />`;
        lightboxCurrent.textContent = index + 1;

        // 첫 사진이면 이전 버튼 숨김
        lightboxPrev.style.opacity = index === 0 ? '0.3' : '1';
        lightboxPrev.style.pointerEvents = index === 0 ? 'none' : 'auto';

        // 마지막 사진이면 다음 버튼 숨김
        lightboxNext.style.opacity = index === photosData.length - 1 ? '0.3' : '1';
        lightboxNext.style.pointerEvents = index === photosData.length - 1 ? 'none' : 'auto';
    }

    // 닫기 버튼
    lightboxClose.addEventListener('click', closeLightbox);
    lightboxOverlay.addEventListener('click', closeLightbox);

    // 이전 버튼
    lightboxPrev.addEventListener('click', function(e) {
        e.stopPropagation();
        showPhoto(currentIndex - 1);
    });

    // 다음 버튼
    lightboxNext.addEventListener('click', function(e) {
        e.stopPropagation();
        showPhoto(currentIndex + 1);
    });

    // 키보드 이벤트 (ESC, 좌우 화살표)
    document.addEventListener('keydown', function(e) {
        if (lightbox.style.display === 'none') return;

        if (e.key === 'Escape') {
            closeLightbox();
        } else if (e.key === 'ArrowLeft') {
            showPhoto(currentIndex - 1);
        } else if (e.key === 'ArrowRight') {
            showPhoto(currentIndex + 1);
        }
    });
}
