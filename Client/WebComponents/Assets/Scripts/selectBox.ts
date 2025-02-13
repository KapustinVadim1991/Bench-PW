// Export functions so they can be called from C# via JS interop

/**
 * Positions the dropdown relative to the select box.
 * @param selectBoxId - The ID of the select box container.
 * @param dropdownId - The ID of the dropdown container.
 * @param dropdownItemsId - The ID of the container holding the dropdown items.
 */
export function positionDropdown(
    selectBoxId: string,
    dropdownId: string,
    dropdownItemsId: string
): void {
    const selectBox = document.getElementById(selectBoxId);
    const dropdown = document.getElementById(dropdownId);
    const dropdownItems = document.getElementById(dropdownItemsId);

    console.log(dropdownItems);
    if (!selectBox || !dropdown || !dropdownItems) return;

    dropdown.style.position = 'absolute';
    dropdown.style.zIndex = '10000';

    const rect = selectBox.getBoundingClientRect();
    const spaceBelow = window.innerHeight - rect.bottom;
    const spaceAbove = rect.top;

    // If a maxHeight is set via inline styles, try to parse it; otherwise, default to 250.
    let dropdownMaxHeight: number;
    if (dropdownItems.style.maxHeight) {
        dropdownMaxHeight = parseInt(dropdownItems.style.maxHeight, 10);
        if (isNaN(dropdownMaxHeight)) {
            dropdownMaxHeight = 250;
        }
    } else {
        dropdownMaxHeight = 250;
    }

    if (spaceBelow < dropdownMaxHeight && spaceAbove > spaceBelow) {
        dropdown.style.top = `${rect.top - dropdownMaxHeight}px`;
    } else {
        dropdown.style.top = `${rect.bottom}px`;
    }
    dropdown.style.left = `${rect.left}px`;
    dropdown.style.width = `${rect.width}px`;
}

/**
 * Global map for storing scroll event handlers.
 * The key is the element ID, and the value is the event listener function.
 */
const eventHandlersMap = new Map<string, EventListener>();

/**
 * Adds a scroll event listener to the element with the specified ID.
 * The function uses dotNetObjectReference to call the C# method 'LoadData'.
 * @param dropdownItemsId - The ID of the element where the scroll event is listened to.
 * @param dotNetObjectReference - The object passed from C# to call JSInvokable methods.
 */
export function addScrollDropdownEventListeners(
    dropdownItemsId: string,
    dotNetObjectReference: any
): void {
    const dropdownItems = document.getElementById(dropdownItemsId);
    if (!dropdownItems || eventHandlersMap.has(dropdownItemsId)) {
        return;
    }

    let isLoading = false;

    // Define the scroll event handler
    const callback = async () => {
        if (
            !isLoading &&
            dropdownItems.scrollTop + dropdownItems.clientHeight >= dropdownItems.scrollHeight - 100
        ) {
            isLoading = true;
            await dotNetObjectReference.invokeMethodAsync('LoadData', true);
            isLoading = false;
        }
    };

    // Store the callback in the map for later removal
    eventHandlersMap.set(dropdownItemsId, callback);
    dropdownItems.addEventListener('scroll', callback);
}

/**
 * Removes the scroll event listener from the element with the specified ID,
 * if such a handler was previously added.
 * @param dropdownItemsId - The ID of the element from which to remove the event handler.
 */
export function resetScrollDropdownEventListeners(dropdownItemsId: string): void {
    if (eventHandlersMap.has(dropdownItemsId)) {
        const callback = eventHandlersMap.get(dropdownItemsId);
        eventHandlersMap.delete(dropdownItemsId);

        const dropdownItems = document.getElementById(dropdownItemsId);
        if (dropdownItems && callback) {
            dropdownItems.removeEventListener('scroll', callback);
        }
    }
}