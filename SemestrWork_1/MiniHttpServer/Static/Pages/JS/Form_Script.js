function SendData(elements)
{
    const data = {};
        
        elements.forEach(elementId => {
            const element = document.getElementById(elementId);
            if (element && element.name) {
                if (element.type === 'checkbox') {
                    data[element.name] = element.checked.toString();
                } else if (element.value !== undefined) {
                    data[element.name] = element.value.toString();
                } else {
                    data[element.name] = element.textContent.trim();
                }
            }
        });
}

// FETCH JSON
async function SendForm(elements) {
    const data = {};
    let dataString = '';
    
    elements.forEach(elementId => {
        const element = document.getElementById(elementId);
        if (element && element.name) {
            if (element.type === 'checkbox') {
                data[element.name] = element.checked.toString();
                dataString += `${element.name}=${element.checked.toString()}`
            } else if (element.value !== undefined) {
                data[element.name] = element.value.toString();
                dataString += `${element.name}=${element.value.toString()}`
            } else {
                data[element.name] = element.textContent.trim();
                dataString += `${element.name}=${element.textContent.trim()}`
            }
        }
        dataString += '&'
    });
    dataString = dataString.substring(0, dataString.length-1);

    try {
        const options = {
            method: methodType,
            headers: {},
            credentials: 'include'
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

        // // Проверяем статус код для редиректа
        // if (response.status >= 300 && response.status < 400) {
        //     const redirectUrl = response.headers.get('Location');
        //     if (redirectUrl) {
        //         window.location.assign(redirectUrl);
        //         // window.location.href = redirectUrl;
        //         return;
        //     }
        // }

        const redirectPage = response.headers.get('Location');
        if (redirectPage)
        {
            window.history.pushState({}, '', redirectPage);
            window.location.href = redirectPage;
            // window.location.assign(redirectPage);
            return;
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

                // document.open();
                // document.write(html);
                // document.close();

                // window.location.assign(urlPath);

                // window.history.pushState({}, '', response.url);
                // window.location.reload();

            } 
            // else {
            //     window.location.reload(urlPath);
            // }
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