$(document).ready(function () {
    function loadCourses(tag = '') {
        var url = '/Course/v1/ListCourses';
        if (tag) {
            url += '/?tag=' + tag;
        }

        $.ajax({
            url: url,
            method: 'GET',
            dataType: 'json',
            success: function (data) {
                console.log('::Success', data);
                displayCourses(data);
            },
            error: function (xhr, status, error) {
                console.error("Error:", error);
                alert("Oops! an error occurred.");
            }
        });
    }
    var defaultImageUrl = '/CoursePhotos/thumbnails/defaultCourseCover.png'; 

    function displayCourses(courses) {
        var courseContainer = $('#course-container');
        courseContainer.empty(); 

        if (courses.length === 0) {
            courseContainer.append('<p>İçerik bulunamadı.</p>');  
            return;
        }

        $.each(courses, function (index, course) {
            var imageUrl = (course.photoUrl === '-' || !course.photoUrl) ? defaultImageUrl : '/CoursePhotos/' + course.photoUrl;

            var courseCard = `
                <div class="course-card">
                    <div class="course-card-img">
                        <img src="${imageUrl}" alt="${course.title}" class="img-fluid">
                    </div>
                    <div class="course-card-body">
                        <h4><a href="/Course/Details/${course.id}" class="course-card-title">${course.title}</a></h4>
                        <p class="course-card-description">${course.description}</p>
                    </div>
                   <hr class="course-card-divider">
                    <div class="course-card-footer">
                        <a id="readNewsButton" href="/Course/Details/${course.id}" class="btn-incele">İncele</a>
                    </div>
                </div>
            `;
            courseContainer.append(courseCard);  
        });
    }

    loadCourses();

    $(".tab-link").click(function (e) {
        e.preventDefault();
        var tag = $(this).data("tab");  

        $(".tab-link").removeClass("active");
        $(this).addClass("active");

        if (tag === "") {
            loadCourses();  
        } else {
            loadCourses(tag); 
        }
    });
});
//Carousel Actions
document.addEventListener("DOMContentLoaded", function () {
    const slides = document.querySelectorAll(".main-carousel-slide");
    const prevButton = document.querySelector(".main-carousel-prev");
    const nextButton = document.querySelector(".main-carousel-next");
    let currentIndex = 0;

    function goToNextSlide() {
        slides[currentIndex].classList.remove("active");
        currentIndex = (currentIndex + 1) % slides.length;
        slides[currentIndex].classList.add("active");
    }

    function goToPreviousSlide() {
        slides[currentIndex].classList.remove("active");
        currentIndex = (currentIndex - 1 + slides.length) % slides.length;
        slides[currentIndex].classList.add("active");
    }

    nextButton.addEventListener("click", goToNextSlide);
    prevButton.addEventListener("click", goToPreviousSlide);

    setInterval(goToNextSlide, 5000);
});

//Tarzan-Player Actions
var player = videojs('tarzan-player');

function changeTarzanPlayerSource(vidSrc) {
    player.src({ type: "video/mp4", src: vidSrc });
    player.play();
}

//Searchbox Enter Event
document.querySelector('.custom-searchbox').addEventListener('submit', function (e) {
    const inputField = document.querySelector('.search-input');
    if (inputField && inputField.value.trim() === "") {
        e.preventDefault();
        alert('Lütfen bir arama terimi girin');
    }
});
