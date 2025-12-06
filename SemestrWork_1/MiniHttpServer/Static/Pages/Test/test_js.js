class DropdownDateRangePicker {
    constructor() {
        this.currentDate = new Date();
        this.startDate = null;
        this.endDate = null;
        this.selectionPhase = 'start';
        this.isOpen = false;
        this.init();
    }

    init() {
        this.renderCalendar();
        this.addEventListeners();
        this.updateDisplay();
    }

    renderCalendar() {
        const year = this.currentDate.getFullYear();
        const month = this.currentDate.getMonth();
        
        document.querySelector('.current-month').textContent = 
            this.getMonthName(month) + ' ' + year;

        const firstDay = new Date(year, month, 1);
        const startingDay = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1;
        const daysInMonth = new Date(year, month + 1, 0).getDate();
        const daysInPrevMonth = new Date(year, month, 0).getDate();
        
        const calendarDays = document.getElementById('calendarDays');
        calendarDays.innerHTML = '';

        // Дни предыдущего месяца
        for (let i = startingDay - 1; i >= 0; i--) {
            const day = daysInPrevMonth - i;
            const dayElement = this.createDayElement(day, 'other-month');
            calendarDays.appendChild(dayElement);
        }

        // Дни текущего месяца
        for (let day = 1; day <= daysInMonth; day++) {
            const date = new Date(year, month, day);
            const dayElement = this.createDayElement(day, 'current-month');
            this.applyRangeStyles(dayElement, date);
            calendarDays.appendChild(dayElement);
        }

        // Дни следующего месяца
        const totalCells = 42;
        const remainingCells = totalCells - (startingDay + daysInMonth);
        for (let day = 1; day <= remainingCells; day++) {
            const dayElement = this.createDayElement(day, 'other-month');
            calendarDays.appendChild(dayElement);
        }
    }

    createDayElement(day, className) {
        const dayElement = document.createElement('div');
        dayElement.className = `day ${className}`;
        dayElement.textContent = day;
        
        if (className === 'current-month') {
            const date = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth(), day);
            dayElement.addEventListener('click', (e) => {
                e.stopPropagation(); // Важно: останавливаем всплытие!
                e.preventDefault();
                this.selectDate(date);
            });
        }
        
        return dayElement;
    }

    selectDate(date) {
        if (this.selectionPhase === 'start') {
            this.startDate = date;
            this.endDate = null;
            this.selectionPhase = 'end';
        } else {
            if (date < this.startDate) {
                this.endDate = this.startDate;
                this.startDate = date;
            } else {
                this.endDate = date;
            }
            this.selectionPhase = 'start';
        }
        
        this.updateDisplay();
        this.renderCalendar();
        
        document.getElementById('startDate').value = 
            this.startDate ? this.startDate.toISOString().split('T')[0] : '';
        document.getElementById('endDate').value = 
            this.endDate ? this.endDate.toISOString().split('T')[0] : '';
    }

    applyRangeStyles(dayElement, date) {
        dayElement.classList.remove('in-range', 'range-start', 'range-end');
        
        if (this.startDate && this.isSameDate(date, this.startDate)) {
            dayElement.classList.add('range-start');
        }
        if (this.endDate && this.isSameDate(date, this.endDate)) {
            dayElement.classList.add('range-end');
        }
        if (this.isDateInRange(date)) {
            dayElement.classList.add('in-range');
        }
    }

    isDateInRange(date) {
        if (!this.startDate || !this.endDate) return false;
        return date > this.startDate && date < this.endDate;
    }

    isSameDate(date1, date2) {
        return date1.getDate() === date2.getDate() &&
               date1.getMonth() === date2.getMonth() &&
               date1.getFullYear() === date2.getFullYear();
    }

    updateDisplay() {
        const selectedDatesElement = document.getElementById('selectedDates');
        const rangeElement = document.getElementById('selectedRange');
        const placeholder = document.querySelector('.date-placeholder');
        
        if (this.startDate && this.endDate) {
            const startStr = this.formatDate(this.startDate);
            const endStr = this.formatDate(this.endDate);
            const displayText = `${startStr} – ${endStr}`;
            
            selectedDatesElement.textContent = displayText;
            rangeElement.textContent = displayText;
            placeholder.style.display = 'none';
        } else if (this.startDate) {
            selectedDatesElement.textContent = this.formatDate(this.startDate);
            rangeElement.textContent = 'Выберите дату окончания';
            placeholder.style.display = 'none';
        } else {
            selectedDatesElement.textContent = '';
            rangeElement.textContent = 'Выберите дату начала';
            placeholder.style.display = 'block';
        }
    }

    formatDate(date) {
        const day = date.getDate().toString().padStart(2, '0');
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        return `${day}.${month}`;
    }

    getMonthName(month) {
        const months = [
            'Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
            'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'
        ];
        return months[month];
    }

    toggle() {
        this.isOpen = !this.isOpen;
        const dropdown = document.getElementById('calendarDropdown');
        const trigger = document.getElementById('dateTrigger');
        
        if (this.isOpen) {
            dropdown.classList.add('show');
            trigger.classList.add('active');
        } else {
            dropdown.classList.remove('show');
            trigger.classList.remove('active');
        }
    }

    close() {
        this.isOpen = false;
        document.getElementById('calendarDropdown').classList.remove('show');
        document.getElementById('dateTrigger').classList.remove('active');
    }

    addEventListeners() {
        document.getElementById('dateTrigger').addEventListener('click', (e) => {
            e.stopPropagation();
            e.preventDefault();
            this.toggle();
        });

        // Предотвращаем всплытие кликов внутри календаря
        document.getElementById('calendarDropdown').addEventListener('click', (e) => {
            e.stopPropagation();
        });

        document.querySelector('.prev').addEventListener('click', (e) => {
            e.stopPropagation();
            e.preventDefault();
            this.currentDate.setMonth(this.currentDate.getMonth() - 1);
            this.renderCalendar();
        });

        document.querySelector('.next').addEventListener('click', (e) => {
            e.stopPropagation();
            e.preventDefault();
            this.currentDate.setMonth(this.currentDate.getMonth() + 1);
            this.renderCalendar();
        });

        document.addEventListener('click', (e) => {
            if (!e.target.closest('.date-input-container')) {
                this.close();
            }
        });

        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                this.close();
            }
        });
    }
}

// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', () => {
    new DropdownDateRangePicker();
});

async function ShowEventPage()
{
    const form = document.createElement('form');

    form.method = 'POST';
    form.action = 'MainEndpoint/showEvent';
    form.style.display = 'none';

    // добавление данных
    // Создаем скрытое поле с JSON
    const jsonInput = document.createElement('input');
    jsonInput.type = 'hidden';
    jsonInput.name = 'requestBody'; // Сервер будет искать это поле

    const tour = document.getElementById('TourId');

    jsonInput.value = JSON.stringify({
        [tour.name]: tour.value.toString()
    });
    form.appendChild(jsonInput);

    document.body.appendChild(form);
    form.submit();
    form.remove();
}

// Функции для управления модальным окном
function openLoginModal() {
    const modal = document.getElementById('loginModal');
    modal.classList.add('active');
    document.body.style.overflow = 'hidden'; // Блокируем прокрутку фона
}

function closeLoginModal() {
    const modal = document.getElementById('loginModal');
    modal.classList.remove('active');
    document.body.style.overflow = ''; // Восстанавливаем прокрутку
    showLoginForm(); // Возвращаем к форме входа
}

function showLoginForm() {
    document.getElementById('loginForm').style.display = 'block';
    document.getElementById('registerForm').style.display = 'none';
}

function showRegisterForm() {
    document.getElementById('loginForm').style.display = 'none';
    document.getElementById('registerForm').style.display = 'block';
}

// Закрытие по клику вне окна
document.getElementById('loginModal').addEventListener('click', function(e) {
    if (e.target === this) {
        closeLoginModal();
    }
});

// Закрытие по ESC
document.addEventListener('keydown', function(e) {
    if (e.key === 'Escape') {
        closeLoginModal();
    }
});

// Обработка форм !!!
document.getElementById('loginForm').addEventListener('submit', function(e) {
    e.preventDefault();
    // Здесь будет логика входа
    console.log('Login form submitted');
});

document.getElementById('registerForm').addEventListener('submit', function(e) {
    e.preventDefault();
    // Здесь будет логика регистрации !!!
    console.log('Register form submitted');
});