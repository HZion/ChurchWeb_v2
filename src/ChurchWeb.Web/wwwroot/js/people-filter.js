// 섬기는 사람들 페이지 필터 기능
document.addEventListener('DOMContentLoaded', function() {
    const filterBtns = document.querySelectorAll('.filter-btn');
    const people = document.querySelectorAll('.person');

    filterBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            const category = this.getAttribute('data-category');

            // 활성 버튼 전환
            filterBtns.forEach(b => b.classList.remove('active'));
            this.classList.add('active');

            // 필터링
            people.forEach(person => {
                const personCategory = person.getAttribute('data-category');
                if (category === 'all' || personCategory === category) {
                    person.style.display = 'block';
                } else {
                    person.style.display = 'none';
                }
            });
        });
    });
});
