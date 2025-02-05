## 📂 **Распределение файлов в ITCSS (Inverted Triangle CSS)**

ITCSS структурирует стили по уровням **от самых общих к самым частным**, чтобы избежать проблем с переопределением, специфичностью и сложностью поддержки CSS.

---
### **🗂 Общая структура папок в ITCSS**
```
/styles
│── /settings     # Переменные и конфигурации
│── /tools        # Миксины, функции, вспомогательные классы
│── /generic      # Reset, normalize, базовые стили
│── /elements     # Стили для HTML-тегов (h1, p, ul, a и т. д.)
│── /objects      # Базовые компоненты (гриды, контейнеры, layout)
│── /components   # UI-элементы (кнопки, карточки, модальные окна)
│── /trumps       # Утилитарные классы (модификаторы, скрытые блоки)
│── main.scss     # Основной файл, собирающий всё воедино
```
🔹 **Чем ниже по структуре – тем выше специфичность стилей.**  
🔹 **Файлы импортируются в `main.scss` в строгом порядке.**

---

## 🔹 **1. `/settings` (Настройки и переменные)**
📌 В этом разделе **нет реального CSS** – только переменные и настройки.  
Пример файла `/styles/settings/_colors.scss`:
```scss
// Основные цвета
$primary-color: #007BFF;
$secondary-color: #6c757d;
$background-color: #f9f9f9;
$text-color: #333;
```
Пример файла `/styles/settings/_typography.scss`:
```scss
// Шрифты
$font-primary: 'Inter', sans-serif;
$font-size-base: 16px;
$line-height-base: 1.5;
```
**Импорт в `main.scss`**:
```scss
@import 'settings/colors';
@import 'settings/typography';
```
🔹 **Преимущества:** все переменные в одном месте, легко менять тему.

---

## 🔹 **2. `/tools` (Функции и миксины)**
📌 Здесь хранятся **Sass-миксины, функции и утилиты**, используемые в других файлах.  
Пример файла `/styles/tools/_mixins.scss`:
```scss
// Миксин для адаптивных медиа-запросов
@mixin responsive($breakpoint) {
    @if $breakpoint == mobile {
        @media (max-width: 600px) { @content; }
    }
    @else if $breakpoint == tablet {
        @media (max-width: 900px) { @content; }
    }
}
```
Пример файла `/styles/tools/_helpers.scss`:
```scss
// Вспомогательные классы
.full-width {
    width: 100%;
}

.center {
    display: flex;
    justify-content: center;
    align-items: center;
}
```
**Импорт в `main.scss`**:
```scss
@import 'tools/mixins';
@import 'tools/helpers';
```
🔹 **Преимущества:** упрощает использование повторяющихся правил.

---

## 🔹 **3. `/generic` (Общие стили: Reset, Normalize)**
📌 Здесь **обнуляются стандартные стили браузера**.  
Пример файла `/styles/generic/_reset.scss`:
```scss
// CSS Reset (сброс стандартных стилей)
* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
}
```
Пример файла `/styles/generic/_normalize.scss`:
```scss
// Нормализация шрифтов и списков
body {
    font-family: $font-primary;
    font-size: $font-size-base;
    line-height: $line-height-base;
    color: $text-color;
}

ul, ol {
    list-style: none;
}
```
**Импорт в `main.scss`**:
```scss
@import 'generic/reset';
@import 'generic/normalize';
```
🔹 **Преимущества:** базовое выравнивание стилей для всех браузеров.

---

## 🔹 **4. `/elements` (Стилизация HTML-тегов)**
📌 Здесь находятся **базовые стили для тегов**, которые не зависят от классов.  
Пример файла `/styles/elements/_typography.scss`:
```scss
// Заголовки
h1, h2, h3, h4 {
    font-weight: bold;
    margin-bottom: 10px;
}

p {
    margin-bottom: 15px;
}
```
Пример файла `/styles/elements/_links.scss`:
```scss
// Ссылки
a {
    text-decoration: none;
    color: $primary-color;
    transition: color 0.3s;
}

a:hover {
    color: darken($primary-color, 10%);
}
```
**Импорт в `main.scss`**:
```scss
@import 'elements/typography';
@import 'elements/links';
```
🔹 **Преимущества:** предсказуемый внешний вид всех элементов.

---

## 🔹 **5. `/objects` (Базовые шаблоны)**
📌 Здесь лежат **переиспользуемые структуры**: контейнеры, гриды, flex-блоки.  
Пример файла `/styles/objects/_grid.scss`:
```scss
// Грид-система
.container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 15px;
}

.row {
    display: flex;
    flex-wrap: wrap;
}

.col {
    flex: 1;
    padding: 10px;
}
```
Пример файла `/styles/objects/_media.scss`:
```scss
// Медиаблок (изображение + текст)
.media {
    display: flex;
    align-items: center;
}

.media img {
    max-width: 100px;
    margin-right: 15px;
}
```
**Импорт в `main.scss`**:
```scss
@import 'objects/grid';
@import 'objects/media';
```
🔹 **Преимущества:** легко использовать в любом проекте.

---

## 🔹 **6. `/components` (Конкретные UI-элементы)**
📌 Здесь хранятся **конкретные стили кнопок, карточек, модалок и других UI-компонентов**.  
Пример файла `/styles/components/_button.scss`:
```scss
// Кнопки
.btn {
    display: inline-block;
    padding: 10px 20px;
    border-radius: 5px;
    font-weight: bold;
    background-color: $primary-color;
    color: white;
    transition: background 0.3s;
}

.btn:hover {
    background-color: darken($primary-color, 10%);
}
```
Пример файла `/styles/components/_card.scss`:
```scss
// Карточка
.card {
    background: white;
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}
```
**Импорт в `main.scss`**:
```scss
@import 'components/button';
@import 'components/card';
```
🔹 **Преимущества:** UI-компоненты полностью переиспользуемые.

---

## 🔹 **7. `/trumps` (Утилитарные классы)**
📌 Здесь хранятся **модификаторы и вспомогательные классы**, которые переопределяют стили.  
Пример файла `/styles/trumps/_utilities.scss`:
```scss
// Скрыватель элементов
.hidden {
    display: none !important;
}

// Быстрое центрирование
.text-center {
    text-align: center;
}

// Цветовые классы
.text-primary {
    color: $primary-color;
}
```
**Импорт в `main.scss`**:
```scss
@import 'trumps/utilities';
```
🔹 **Преимущества:** минимальные CSS-хелперы для быстрого стилизации.

---

## 🎯 **Заключение**
Теперь твой CSS-фреймворк:
✅ **Структурирован**  
✅ **Легко поддерживается**  
✅ **Гибко расширяется**

🚀 **Используй ITCSS в своих проектах, чтобы улучшить качество и удобство работы с CSS!**