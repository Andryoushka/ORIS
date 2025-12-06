// =============================================
// ПЕРЕМЕННЫЕ И СОСТОЯНИЕ
// =============================================

let currentTours = [];
let filteredTours = [];
let currentPage = 1;
const toursPerPage = 10;
let currentSelectedTour = null;

// Элементы DOM
const toursList = document.getElementById('toursList');
const searchInput = document.getElementById('searchInput');
const searchBtn = document.getElementById('searchBtn');
const statusFilter = document.getElementById('statusFilter');
const typeFilter = document.getElementById('typeFilter');
const resetFiltersBtn = document.getElementById('resetFiltersBtn');
const prevPageBtn = document.getElementById('prevPageBtn');
const nextPageBtn = document.getElementById('nextPageBtn');
const pageNumbers = document.getElementById('pageNumbers');
const showingCount = document.getElementById('showingCount');
const totalCount = document.getElementById('totalCount');

// Статистика
const totalToursCount = document.getElementById('totalToursCount');
const activeToursCount = document.getElementById('activeToursCount');
const draftToursCount = document.getElementById('draftToursCount');
const archivedToursCount = document.getElementById('archivedToursCount');

// Модальное окно
const actionModal = document.getElementById('actionModal');
const modalTitle = document.getElementById('modalTitle');

// =============================================
// МОК ДАННЫХ (ЗАМЕНИТЬ НА РЕАЛЬНЫЙ API)
// =============================================

const mockTours = [
    {
        id: 1,
        name: "Экскурсия по Красной площади",
        image: "",
        type: "Экскурсия",
        rating: 4.8,
        price: 2500,
        status: "active",
        location: "Москва, Красная площадь",
        createdAt: "2024-01-15"
    },
    {
        id: 2,
        name: "Тур по Золотому кольцу",
        image: "",
        type: "Тур",
        rating: 4.9,
        price: 15600,
        status: "active",
        location: "Московская область",
        createdAt: "2024-01-10"
    }
];

// =============================================
// ФУНКЦИИ ДЛЯ РАБОТЫ С ДАННЫМИ
// =============================================

/**
 * Загружает туры из базы данных (мок)
 */
async function loadTours() {
    try {
        // Имитация загрузки с сервера
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        // В реальном приложении здесь будет fetch запрос
        currentTours = mockTours;
        applyFilters();
        updateStatistics();
        
    } catch (error) {
        console.error('Ошибка загрузки туров:', error);
        showError('Не удалось загрузить список туров');
    }
}

/**
 * Применяет фильтры и поиск
 */
function applyFilters() {
    let result = [...currentTours];
    
    // Поиск по названию
    const searchTerm = searchInput.value.toLowerCase().trim();
    if (searchTerm) {
        result = result.filter(tour => 
            tour.name.toLowerCase().includes(searchTerm)
        );
    }
    
    // Фильтр по статусу
    const statusValue = statusFilter.value;
    if (statusValue) {
        result = result.filter(tour => tour.status === statusValue);
    }
    
    // Фильтр по типу
    const typeValue = typeFilter.value;
    if (typeValue) {
        result = result.filter(tour => tour.type === typeValue);
    }
    
    filteredTours = result;
    currentPage = 1;
    renderToursList();
    updatePagination();
}

/**
 * Обновляет статистику
 */
function updateStatistics() {
    const total = currentTours.length;
    const active = currentTours.filter(tour => tour.status === 'active').length;
    const draft = currentTours.filter(tour => tour.status === 'draft').length;
    const archived = currentTours.filter(tour => tour.status === 'archived').length;
    
    totalToursCount.textContent = total;
    activeToursCount.textContent = active;
    draftToursCount.textContent = draft;
    archivedToursCount.textContent = archived;
}

// =============================================
// ФУНКЦИИ ДЛЯ ОТОБРАЖЕНИЯ
// =============================================

/**
 * Отображает список туров
 */
function renderToursList() {
    if (filteredTours.length === 0) {
        toursList.innerHTML = '<div class="empty_message">Туры не найдены</div>';
        return;
    }
    
    const startIndex = (currentPage - 1) * toursPerPage;
    const endIndex = startIndex + toursPerPage;
    const toursToShow = filteredTours.slice(startIndex, endIndex);
    
    // <img src="${tour.image}" alt="${tour.name}" onerror="this.src='./default-tour.jpg'">
    toursList.innerHTML = toursToShow.map(tour => `
        <div class="tour_row" data-tour-id="${tour.id}">
            <div class="col_image">
            </div>
            <div class="col_name">${tour.name}</div>
            <div class="col_type">${tour.type}</div>
            <div class="col_rating">
                <span class="rating_stars">${getStarRating(tour.rating)}</span>
                <span>${tour.rating}</span>
            </div>
            <div class="col_price">${formatPrice(tour.price)} ₽</div>
            <div class="col_status status-${tour.status}">
                ${getStatusText(tour.status)}
            </div>
            <div class="col_actions">
                <button type="button" class="small-btn" onclick="openTourActions(${tour.id})" title="Действия">⚙️</button>
                <button type="button" class="small-btn" onclick="editTour(${tour.id})" title="Редактировать">✏️</button>
                <button type="button" class="small-btn" onclick="viewTour(${tour.id})" title="Посмотреть">👁️</button>
            </div>
        </div>
    `).join('');
    
    updateShowingCount();
}

/**
 * Возвращает звезды рейтинга
 */
function getStarRating(rating) {
    const fullStars = Math.floor(rating);
    const halfStar = rating % 1 >= 0.5;
    const emptyStars = 5 - fullStars - (halfStar ? 1 : 0);
    
    return '★'.repeat(fullStars) + (halfStar ? '½' : '') + '☆'.repeat(emptyStars);
}

/**
 * Форматирует цену
 */
function formatPrice(price) {
    return price.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ' ');
}

/**
 * Возвращает текст статуса
 */
function getStatusText(status) {
    const statusMap = {
        'active': 'Активен',
        'draft': 'Черновик',
        'archived': 'Архив'
    };
    return statusMap[status] || status;
}

/**
 * Обновляет счетчик показанных туров
 */
function updateShowingCount() {
    const startIndex = (currentPage - 1) * toursPerPage + 1;
    const endIndex = Math.min(startIndex + toursPerPage - 1, filteredTours.length);
    
    showingCount.textContent = `${startIndex}-${endIndex}`;
    totalCount.textContent = filteredTours.length;
}

// =============================================
// ПАГИНАЦИЯ
// =============================================

/**
 * Обновляет пагинацию
 */
function updatePagination() {
    const totalPages = Math.ceil(filteredTours.length / toursPerPage);
    
    // Кнопки вперед/назад
    prevPageBtn.disabled = currentPage === 1;
    nextPageBtn.disabled = currentPage === totalPages || totalPages === 0;
    
    // Номера страниц
    pageNumbers.innerHTML = '';
    
    if (totalPages === 0) return;
    
    // Показываем максимум 5 страниц
    let startPage = Math.max(1, currentPage - 2);
    let endPage = Math.min(totalPages, startPage + 4);
    
    if (endPage - startPage < 4) {
        startPage = Math.max(1, endPage - 4);
    }
    
    for (let i = startPage; i <= endPage; i++) {
        const pageBtn = document.createElement('button');
        pageBtn.className = `page_number ${i === currentPage ? 'active' : ''}`;
        pageBtn.textContent = i;
        pageBtn.onclick = () => goToPage(i);
        pageNumbers.appendChild(pageBtn);
    }
}

/**
 * Переход на страницу
 */
function goToPage(page) {
    currentPage = page;
    renderToursList();
    updatePagination();
}

// =============================================
// ДЕЙСТВИЯ С ТУРАМИ
// =============================================

/**
 * Открывает модальное окно действий
 */
function openTourActions(tourId) {
    const tour = currentTours.find(t => t.id === tourId);
    if (!tour) return;
    
    currentSelectedTour = tour;
    modalTitle.textContent = `Действия с туром: ${tour.name}`;
    actionModal.style.display = 'block';
}

/**
 * Редактирование тура
 */
function editTour(tourId) {
    const tour = currentTours.find(t => t.id === tourId);
    if (tour) {
        // В реальном приложении - переход на страницу редактирования
        alert(`Редактирование тура: ${tour.name}`);
        console.log('Переход к редактированию тура:', tour);
    }
}

/**
 * Просмотр тура
 */
function viewTour(tourId) {
    const tour = currentTours.find(t => t.id === tourId);
    if (tour) {
        // В реальном приложении - переход на страницу просмотра
        alert(`Просмотр тура: ${tour.name}`);
        console.log('Переход к просмотру тура:', tour);
    }
}

/**
 * Удаление тура
 */
function deleteTour() {
    if (!currentSelectedTour) return;
    
    if (confirm(`Вы уверены, что хотите удалить тур "${currentSelectedTour.name}"?`)) {
        // В реальном приложении - запрос к API
        currentTours = currentTours.filter(t => t.id !== currentSelectedTour.id);
        applyFilters();
        updateStatistics();
        actionModal.style.display = 'none';
        showSuccess('Тур успешно удален');
    }
}

/**
 * Изменение статуса тура
 */
function changeTourStatus() {
    if (!currentSelectedTour) return;
    
    const newStatus = prompt(
        `Введите новый статус для тура "${currentSelectedTour.name}":\n(active, draft, archived)`,
        currentSelectedTour.status
    );
    
    if (newStatus && ['active', 'draft', 'archived'].includes(newStatus)) {
        // В реальном приложении - запрос к API
        currentSelectedTour.status = newStatus;
        applyFilters();
        updateStatistics();
        actionModal.style.display = 'none';
        showSuccess('Статус тура обновлен');
    }
}

// =============================================
// УТИЛИТЫ
// =============================================

/**
 * Показывает сообщение об ошибке
 */
function showError(message) {
    // В реальном приложении можно использовать toast уведомления
    alert(`Ошибка: ${message}`);
}

/**
 * Показывает сообщение об успехе
 */
function showSuccess(message) {
    // В реальном приложении можно использовать toast уведомления
    alert(`✅ ${message}`);
}

/**
 * Сбрасывает фильтры
 */
function resetFilters() {
    searchInput.value = '';
    statusFilter.value = '';
    typeFilter.value = '';
    applyFilters();
}

// =============================================
// ИНИЦИАЛИЗАЦИЯ
// =============================================

function initializeEventHandlers() {
    // Поиск и фильтры
    searchBtn.addEventListener('click', applyFilters);
    searchInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') applyFilters();
    });
    statusFilter.addEventListener('change', applyFilters);
    typeFilter.addEventListener('change', applyFilters);
    resetFiltersBtn.addEventListener('click', resetFilters);
    
    // Пагинация
    prevPageBtn.addEventListener('click', () => goToPage(currentPage - 1));
    nextPageBtn.addEventListener('click', () => goToPage(currentPage + 1));
    
    // Модальное окно
    document.querySelector('.close').addEventListener('click', () => {
        actionModal.style.display = 'none';
    });
    
    // Действия в модальном окне
    document.getElementById('editTourBtn').addEventListener('click', () => {
        if (currentSelectedTour) editTour(currentSelectedTour.id);
        actionModal.style.display = 'none';
    });
    
    document.getElementById('viewTourBtn').addEventListener('click', () => {
        if (currentSelectedTour) viewTour(currentSelectedTour.id);
        actionModal.style.display = 'none';
    });
    
    document.getElementById('deleteTourBtn').addEventListener('click', deleteTour);
    document.getElementById('changeStatusBtn').addEventListener('click', changeTourStatus);
    
    // Закрытие модального окна по клику вне
    window.addEventListener('click', (e) => {
        if (e.target === actionModal) {
            actionModal.style.display = 'none';
        }
    });
}

// =============================================
// ЗАПУСК ПРИЛОЖЕНИЯ
// =============================================

document.addEventListener('DOMContentLoaded', function() {
    initializeEventHandlers();
    loadTours();
    console.log('Tours List Page initialized successfully!');
});