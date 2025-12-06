// =============================================
// ПЕРЕМЕННЫЕ И СОСТОЯНИЕ
// =============================================

// Текущие редактируемые элементы
let currentEditingDay = null;
let currentEditingPoint = null;
let currentEditingPicture = null;
let isEditingPicture = false;

// Элементы предпросмотра карточки
const previewImage = document.getElementById('previewImage');
const previewName = document.getElementById('previewName');
const previewLocation = document.getElementById('previewLocation');
const previewType = document.getElementById('previewType');
const previewMetro = document.getElementById('previewMetro');
const previewRating = document.getElementById('previewRating');

// Элементы редактирования карточки
const cardImage = document.getElementById('cardImage');
const cardName = document.getElementById('cardName');
const cardLocation = document.getElementById('cardLocation');
const cardType = document.getElementById('cardType');
const cardMetro = document.getElementById('cardMetro');
const cardRating = document.getElementById('cardRating');
const nameCounter = document.getElementById('nameCounter');

// Дополнительные элементы карточки
const cardPrice = document.getElementById('cardPrice');
const cardNights = document.getElementById('cardNights');
const cardTourType = document.getElementById('cardTourType');
const cardFlight = document.getElementById('cardFlight');
const cardAccommodation = document.getElementById('cardAccommodation');
const cardMeals = document.getElementById('cardMeals');
const cardWeekend = document.getElementById('cardWeekend');
const cardBudget = document.getElementById('cardBudget');
const cardChildren = document.getElementById('cardChildren');

// =============================================
// ФУНКЦИИ УТИЛИТЫ
// =============================================

/**
 * Обновляет нумерацию дней в программе тура
 */
function updateDayNumbers() {
    const days = document.querySelectorAll('.tour_pr');
    days.forEach((day, index) => {
        const dayNumber = day.querySelector('.circle');
        dayNumber.textContent = (index + 1).toString().padStart(2, '0');
        day.setAttribute('data-day', index + 1);
    });
}

/**
 * Обновляет индексы изображений в галерее
 */
function updatePictureIndexes() {
    const pictures = document.querySelectorAll('.picture-item');
    pictures.forEach((picture, index) => {
        picture.setAttribute('data-index', index);
    });
}

/**
 * Оптимизирует расположение изображений в сетке галереи
 */
function optimizePictureGrid() {
    const grid = document.getElementById('tourPictureGrid');
    const pictures = Array.from(grid.children);
    
    const bigPictures = pictures.filter(p => p.classList.contains('big-picture'));
    const smallPictures = pictures.filter(p => p.classList.contains('small-picture'));
    
    grid.innerHTML = '';
    
    bigPictures.slice(0, 2).forEach(picture => grid.appendChild(picture));
    smallPictures.forEach(picture => grid.appendChild(picture));
    
    updatePictureIndexes();
}

// =============================================
// ФУНКЦИИ ДЛЯ КАРТОЧКИ ТУРА
// =============================================

/**
 * Обновляет предпросмотр карточки в реальном времени
 */
function initializeCardPreview() {
    // Изображение
    cardImage.addEventListener('input', function() {
        const url = this.value.trim();
        if (url) {
            previewImage.src = url;
            const smallPreview = document.getElementById('cardImagePreview');
            smallPreview.innerHTML = `<img src="${url}" alt="Предпросмотр">`;
        }
    });

    // Название
    cardName.addEventListener('input', function() {
        previewName.textContent = this.value || 'Название тура';
        nameCounter.textContent = this.value.length;
    });

    // Местоположение
    cardLocation.addEventListener('input', function() {
        previewLocation.textContent = this.value || 'Местоположение';
    });

    // Тип тура
    cardType.addEventListener('change', function() {
        previewType.textContent = this.value;
    });

    // Метро
    cardMetro.addEventListener('input', function() {
        previewMetro.textContent = this.value || 'Ближайшее метро';
    });

    // Рейтинг
    cardRating.addEventListener('input', function() {
        previewRating.textContent = this.value;
    });
}

/**
 * Предпросмотр изображения в отдельном окне
 */
function initializeImagePreview() {
    document.getElementById('previewImageBtn').addEventListener('click', function() {
        const url = cardImage.value.trim();
        if (url) {
            const newWindow = window.open('', '_blank');
            newWindow.document.write(`
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Просмотр изображения</title>
                    <style>
                        body { 
                            margin: 0; 
                            padding: 20px; 
                            display: flex; 
                            justify-content: center; 
                            align-items: center; 
                            min-height: 100vh;
                            background: #f5f5f5;
                        }
                        img { 
                            max-width: 90vw; 
                            max-height: 90vh; 
                            border-radius: 8px;
                            box-shadow: 0 4px 20px rgba(0,0,0,0.3);
                        }
                    </style>
                </head>
                <body>
                    <img src="${url}" alt="Просмотр изображения" onerror="alert('Ошибка загрузки изображения')">
                </body>
                </html>
            `);
        } else {
            alert('Введите ссылку на изображение');
        }
    });
}

/**
 * Сброс формы карточки
 */
function initializeCardReset() {
    document.getElementById('resetCardBtn').addEventListener('click', function() {
        if (confirm('Вы уверены, что хотите сбросить все изменения?')) {
            document.getElementById('cardEditorForm').reset();
            
            // Сброс предпросмотра
            previewImage.src = './Цой.png';
            previewName.textContent = 'Название тура';
            previewLocation.textContent = 'Местоположение';
            previewType.textContent = 'Музей';
            previewMetro.textContent = 'Ближайшее метро';
            previewRating.textContent = '4.8';
            nameCounter.textContent = '0';
            
            // Сброс дополнительных полей
            cardPrice.value = '';
            cardNights.value = '2';
            cardTourType.value = 'Групповой';
            cardFlight.checked = false;
            cardAccommodation.checked = true;
            cardMeals.checked = false;
            cardWeekend.checked = false;
            cardBudget.checked = false;
            cardChildren.checked = false;
            
            // Сброс предпросмотра изображения
            document.getElementById('cardImagePreview').innerHTML = '<span>Предпросмотр изображения</span>';
        }
    });
}

/**
 * Сохранение карточки тура
 */
function initializeCardSave() {
    document.getElementById('saveCardBtn').addEventListener('click', function() {
        const imageUrl = cardImage.value.trim();
        const name = cardName.value.trim();
        const location = cardLocation.value.trim();
        
        // Валидация
        if (!imageUrl) {
            alert('Пожалуйста, добавьте изображение для карточки');
            return;
        }
        
        if (!name) {
            alert('Пожалуйста, введите название тура');
            return;
        }
        
        if (!location) {
            alert('Пожалуйста, укажите местоположение');
            return;
        }

        // Сбор данных
        const cardData = {
            // Основные данные
            image: imageUrl,
            name: name,
            location: location,
            type: cardType.value,
            metro: cardMetro.value,
            rating: cardRating.value,
            
            // Дополнительные данные
            price: cardPrice.value ? parseInt(cardPrice.value) : null,
            nights: parseInt(cardNights.value),
            tourType: cardTourType.value,
            features: {
                flight: cardFlight.checked,
                accommodation: cardAccommodation.checked,
                meals: cardMeals.checked,
                weekend: cardWeekend.checked,
                budget: cardBudget.checked,
                children: cardChildren.checked
            }
        };
        
        console.log('Данные карточки для сохранения:', cardData);
        alert('Карточка тура успешно сохранена!');
        
        // Отправка на сервер (раскомментировать при необходимости)
        /*
        fetch('/api/save-tour-card', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(cardData)
        });
        */
    });
}

// =============================================
// ФУНКЦИИ ДЛЯ ГАЛЕРЕИ ИЗОБРАЖЕНИЙ
// =============================================

/**
 * Привязывает обработчики событий к изображению
 */
function attachPictureEventListeners(pictureElement) {
    // Редактирование изображения
    pictureElement.querySelector('.edit-picture-btn').addEventListener('click', function() {
        const img = pictureElement.querySelector('img');
        document.getElementById('pictureUrl').value = img.src;
        document.getElementById('pictureAlt').value = img.alt;
        document.getElementById('pictureSize').value = pictureElement.classList.contains('big-picture') ? 'big' : 'small';
        
        const preview = document.getElementById('imagePreview');
        preview.innerHTML = `<img src="${img.src}" alt="Предпросмотр">`;
        
        currentEditingPicture = pictureElement;
        isEditingPicture = true;
        document.getElementById('pictureModal').style.display = 'block';
    });

    // Удаление изображения
    pictureElement.querySelector('.delete-picture-btn').addEventListener('click', function() {
        if (confirm('Вы уверены, что хотите удалить это изображение?')) {
            pictureElement.remove();
            updatePictureIndexes();
            optimizePictureGrid();
            reindexPictureInputs();
        }
    });
}

/**
 * Переиндексирует скрытые инпуты после удаления изображения
 */
function reindexPictureInputs() {
    const pictures = document.querySelectorAll('.picture-item');
    pictures.forEach((picture, index) => {
        picture.setAttribute('data-index', index);
        
        // Обновляем name и id у img и input
        const img = picture.querySelector('img');
        const input = picture.querySelector('input[type="hidden"]');
        
        if (img && input) {
            img.name = `PageImage_${index}`;
            input.name = `PageImage_${index}`;
            input.id = `pageImage_${index}_id`;
        }
    });
}

/**
 * Инициализация функционала галереи
 */
function initializeGallery() {
    // Предпросмотр изображения в модальном окне
    document.getElementById('pictureUrl').addEventListener('input', function() {
        const preview = document.getElementById('imagePreview');
        const url = this.value.trim();
        
        if (url) {
            preview.innerHTML = `<img src="${url}" alt="Предпросмотр" onerror="this.style.display='none'; preview.innerHTML='<span>Ошибка загрузки изображения</span>'">`;
        } else {
            preview.innerHTML = '<span>Предпросмотр появится здесь</span>';
        }
    });

    // Добавление нового изображения
    document.querySelector('.add-picture-btn').addEventListener('click', function() {
        document.getElementById('pictureUrl').value = '';
        document.getElementById('pictureAlt').value = '';
        document.getElementById('pictureSize').value = 'big';
        document.getElementById('imagePreview').innerHTML = '<span>Предпросмотр появится здесь</span>';
        currentEditingPicture = null;
        isEditingPicture = false;
        document.getElementById('pictureModal').style.display = 'block';
    });

    // Сохранение изображения
    document.getElementById('savePictureBtn').addEventListener('click', function() {
        const url = document.getElementById('pictureUrl').value.trim();
        const alt = document.getElementById('pictureAlt').value.trim();
        const size = document.getElementById('pictureSize').value;
        
        if (!url) {
            alert('Пожалуйста, введите ссылку на изображение');
            return;
        }

        if (isEditingPicture && currentEditingPicture) {
            // Редактирование существующего изображения
            const img = currentEditingPicture.querySelector('img');
            img.src = url;
            img.alt = alt || 'Изображение тура';
            
            // Обновление размера
            currentEditingPicture.classList.remove('small-picture', 'big-picture');
            currentEditingPicture.classList.add(size === 'big' ? 'big-picture' : 'small-picture');

            // Обновление инпута
            const hiddenInput = currentEditingPicture.querySelector('input[type="hidden"]');
            if (hiddenInput) {
                hiddenInput.value = url;
            }
        } else {
            // Добавление нового изображения
            const grid = document.getElementById('tourPictureGrid');
            const pictureCount = grid.children.length;
            
            const position = pictureCount < 2 ? 'big' : 'small';
            const newPicture = document.createElement('div');
            newPicture.className = `picture-item ${size === 'big' ? 'big-picture' : 'small-picture'}`;
            newPicture.setAttribute('data-index', pictureCount);
            
            newPicture.innerHTML = `
                <img name="PageImage_${pictureCount}" src="${url}" alt="${alt || 'Изображение тура'}">
                <input name="PageImage_${pictureCount}" type="hidden" id="pageImage_${pictureCount}_id" value="${url}">
                
                <div class="picture-actions">
                    <button type="button" class="small-btn edit-picture-btn">✏️</button>
                    <button type="button" class="small-btn delete-picture-btn">🗑️</button>
                </div>
            `;
            
            grid.appendChild(newPicture);
            attachPictureEventListeners(newPicture);
            updatePictureIndexes();
            optimizePictureGrid();
        }
        
        document.getElementById('pictureModal').style.display = 'none';
    });

    // Отмена редактирования изображения
    document.getElementById('cancelPictureBtn').addEventListener('click', function() {
        document.getElementById('pictureModal').style.display = 'none';
    });
}

// =============================================
// ФУНКЦИИ ДЛЯ ПРОГРАММЫ ТУРА
// =============================================

/**
 * Привязывает обработчики событий к элементам дня
 */
function attachDayEventListeners(dayElement) {
    // Редактирование дня
    dayElement.querySelector('.edit-day-btn').addEventListener('click', function() {
        const dayTitle = dayElement.querySelector('.tour_day span:last-child');
        document.getElementById('dayTitle').value = dayTitle.textContent;
        currentEditingDay = dayTitle;
        document.getElementById('dayModal').style.display = 'block';
    });

    // Удаление дня
    dayElement.querySelector('.delete-day-btn').addEventListener('click', function() {
        if (confirm('Вы уверены, что хотите удалить этот день?')) {
            dayElement.remove();
            updateDayNumbers();
        }
    });

    // Добавление задачи
    dayElement.querySelector('.add-point-btn').addEventListener('click', function() {
        const pointsList = dayElement.querySelector('.points-list');
        const newPoint = document.createElement('li');
        newPoint.innerHTML = `
            <span>Новая задача</span>
            <button type="button" class="small-btn edit-point-btn">✏️</button>
            <button type="button" class="small-btn delete-point-btn">🗑️</button>
        `;
        pointsList.appendChild(newPoint);
        attachPointEventListeners(newPoint);
    });

    // Привязка обработчиков к существующим задачам
    dayElement.querySelectorAll('.points-list li').forEach(point => {
        attachPointEventListeners(point);
    });
}

/**
 * Привязывает обработчики событий к задачам
 */
function attachPointEventListeners(pointElement) {
    // Редактирование задачи
    pointElement.querySelector('.edit-point-btn').addEventListener('click', function() {
        const pointText = pointElement.querySelector('span');
        document.getElementById('pointText').value = pointText.textContent;
        currentEditingPoint = pointText;
        document.getElementById('pointModal').style.display = 'block';
    });

    // Удаление задачи
    pointElement.querySelector('.delete-point-btn').addEventListener('click', function() {
        pointElement.remove();
    });
}

/**
 * Инициализация функционала программы тура
 */
function initializeTourProgram() {
    // Добавление нового дня
    document.getElementById('addDayBtn').addEventListener('click', function() {
        const container = document.getElementById('tourProgramContainer');
        const dayCount = container.children.length + 1;
        
        const newDay = document.createElement('div');
        newDay.className = 'tour_pr';
        newDay.setAttribute('data-day', dayCount);
        
        newDay.innerHTML = `
            <div class="tour_day">
                <div class="circle">${dayCount.toString().padStart(2, '0')}</div>
                <span>день</span>
                <div class="day-actions">
                    <button type="button" class="small-btn edit-day-btn">✏️</button>
                    <button type="button" class="small-btn delete-day-btn">🗑️</button>
                </div>
            </div>
            <div class="tour_points">
                <div class="points-header">
                    <span>Задачи дня:</span>
                    <button type="button" class="small-btn add-point-btn">+ Добавить задачу</button>
                </div>
                <ul class="points-list"></ul>
            </div>
        `;
        
        container.appendChild(newDay);
        attachDayEventListeners(newDay);
    });

    // Сохранение изменений дня
    document.getElementById('saveDayBtn').addEventListener('click', function() {
        if (currentEditingDay) {
            currentEditingDay.textContent = document.getElementById('dayTitle').value || 'день';
            document.getElementById('dayModal').style.display = 'none';
        }
    });

    // Сохранение изменений задачи
    document.getElementById('savePointBtn').addEventListener('click', function() {
        if (currentEditingPoint) {
            currentEditingPoint.textContent = document.getElementById('pointText').value || 'Новая задача';
            document.getElementById('pointModal').style.display = 'none';
        }
    });
}

// =============================================
// ФУНКЦИИ ДЛЯ ОПИСАНИЯ ТУРА
// =============================================

/**
 * Инициализация функционала описания тура
 */
function initializeTourDescription() {
    // Редактирование описания
    document.querySelector('.edit-description-btn').addEventListener('click', function() {
        const descriptionElement = document.getElementById('tourDescription');
        document.getElementById('descriptionText').value = descriptionElement.textContent;
        document.getElementById('descriptionModal').style.display = 'block';
    });

    // Сохранение описания
    document.getElementById('saveDescriptionBtn').addEventListener('click', function() {
        const descriptionElement = document.getElementById('tourDescription');
        const newDescription = document.getElementById('descriptionText').value;
        
        if (newDescription.trim()) {
            descriptionElement.textContent = newDescription;
        }
        
        document.getElementById('descriptionModal').style.display = 'none';
    });

    // Отмена редактирования описания
    document.getElementById('cancelDescriptionBtn').addEventListener('click', function() {
        document.getElementById('descriptionModal').style.display = 'none';
    });
}

// =============================================
// ОБЩИЕ ФУНКЦИИ
// =============================================

/**
 * Закрытие модальных окон
 */
function initializeModalClose() {
    // Закрытие по кнопке
    document.querySelectorAll('.close').forEach(closeBtn => {
        closeBtn.addEventListener('click', function() {
            this.closest('.modal').style.display = 'none';
        });
    });

    // Закрытие по клику вне окна
    window.addEventListener('click', function(event) {
        if (event.target.classList.contains('modal')) {
            event.target.style.display = 'none';
        }
    });
}

/**
 * Инициализация всех обработчиков событий
 */
function initializeEventHandlers() {
    initializeCardPreview();
    initializeImagePreview();
    initializeCardReset();
    initializeCardSave();
    initializeGallery();
    initializeTourProgram();
    initializeTourDescription();
    initializeModalClose();
}

/**
 * Инициализация существующих элементов
 */
function initializeExistingElements() {
    // Существующие дни программы
    document.querySelectorAll('.tour_pr').forEach(day => {
        attachDayEventListeners(day);
    });

    // Существующие изображения галереи
    document.querySelectorAll('.picture-item').forEach(picture => {
        attachPictureEventListeners(picture);
    });

    // Инициализация счетчика символов
    nameCounter.textContent = cardName.value.length;
}

// =============================================
// ИНИЦИАЛИЗАЦИЯ ПРИ ЗАГРУЗКЕ СТРАНИЦЫ
// =============================================

document.addEventListener('DOMContentLoaded', function() {
    initializeEventHandlers();
    initializeExistingElements();
    console.log('EventPage initialized successfully!');
});

// =============================================
// ФУНКЦИЯ ДЛЯ ФОРМИРОВАНИЯ JSON ПРОГРАММЫ ТУРА
// =============================================

/**
 * Формирует JSON объект с программой тура
 * @returns {Object} Объект с днями и задачами
 */
function getTourProgramJSON() {
    const days = [];
    
    // Получаем все элементы дней
    const dayElements = document.querySelectorAll('.tour_pr');
    
    dayElements.forEach((dayElement, index) => {
        const dayId = parseInt(dayElement.getAttribute('data-day')) || (index + 1);
        
        // Получаем все задачи для этого дня
        const taskElements = dayElement.querySelectorAll('.points-list li');
        const tasks = Array.from(taskElements).map(taskElement => {
            return taskElement.querySelector('span').textContent.trim();
        });
        
        days.push({
            Id: dayId,
            Tasks: tasks
        });
    });

    return JSON.stringify(days);

    return {
        Days: days
    };
}

async function SaveEventPage() {
    const desc = document.getElementById('tourDescription').textContent;
    document.getElementById('description_id').value = desc;
    const tp = getTourProgramJSON();
    document.getElementById('tourProgram_id').value = tp;

    document.getElementById('eventPage').submit();
}

// FETCH JSON
async function SaveEvent(urlPath, methodType, elements) {
    const data = {};
    
    elements.forEach(elementId => {
        const element = document.getElementById(elementId);
        const fieldName = element.getAttribute('name') || element.name || elementId;
        if (element && fieldName) {
            if (element.type === 'checkbox') {
                data[fieldName] = element.checked.toString();
            } else if (element.value !== undefined) {
                data[fieldName] = element.value.toString();
            } else {
                data[fieldName] = element.textContent.trim();
            }
        }
    });
    data['TourProgram'] = getTourProgramJSON();

    try {
        const options = {
            method: methodType,
            headers: {}
        };

        // Для GET - добавляем данные в URL как query параметры
        if (methodType.toUpperCase() === 'GET') {
            const params = new URLSearchParams(data);
            urlPath = `${urlPath}?${params}`;
        } 
        // Для POST, PUT, PATCH - добавляем body и Content-Type
        else {
            options.headers['Content-Type'] = 'application/json';
            options.body = JSON.stringify(data);
        }

        const response = await fetch(urlPath, options);
        const contentt = response.headers.get('content-type');

        // Проверяем статус код для редиректа
        if (response.status >= 300 && response.status < 400) {
            const redirectUrl = response.headers.get('Location');
            if (redirectUrl) {
                window.location.href = redirectUrl;
                return;
            }
        }

        // Обрабатываем успешные ответы
        if (response.ok) {
            console.log('Request successful, status:', response.status);
            const contentType = response.headers.get('content-type');
            // Вариант 1: Просто обновить страницу
            // window.location.reload();
            
            // Вариант 2: Если сервер возвращает HTML контент
            if (contentType && contentType.includes('text/html')) {
                const html = await response.text();
                // document.documentElement.innerHTML = html;
                document.open();
                document.write(html);
                document.close();

                // window.history.pushState({}, '', fullUrl);
                // window.location.reload();

            } else {
                window.location.reload();
            }
        }
            
            // Вариант 3: Обновить только определенную часть страницы
            // if (contentType && contentType.includes('application/json')) {
            //     const result = await response.json();
            //     // Обновить только нужные элементы на странице
            //     updatePageContent(result);
            // } else {
            //     window.location.reload();
            // }

        // Обрабатываем успешные ответы
        // if (response.ok) {
        //     const result = await response.json();
        //     console.log('Success:', result);
        //     return result;
        // } else {
        //     throw new Error(`HTTP error! status: ${response.status}`);
        // }

    } catch (error) {
        console.error('Error:', error);
        throw error;
    }
}