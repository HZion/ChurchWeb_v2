// 교회소식 분류 필터 기능
// 전체 / 교회소식 / 교우소식 클라이언트 필터

document.addEventListener('DOMContentLoaded', function() {
    const filterBtns = document.querySelectorAll('.filter-btn');
    const noticeItems = document.querySelectorAll('.notice-item');

    filterBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            const category = this.getAttribute('data-category');

            // 활성 버튼 전환
            filterBtns.forEach(b => b.classList.remove('active'));
            this.classList.add('active');

            // 필터링 (필독은 항상 표시)
            noticeItems.forEach(item => {
                const itemCategory = item.getAttribute('data-category');
                const isPinned = item.classList.contains('pinned');

                if (category === 'all' || itemCategory === category || isPinned) {
                    item.style.display = 'table-row';
                } else {
                    item.style.display = 'none';
                }
            });
        });
    });
});
