(function(){
    window.QuarkDatePicker = {
        attach: function (element) {
            // No-op for now; custom calendar is rendered by the component.
            if (!element) return null;
            element.setAttribute('autocomplete', 'off');
            return null;
        },
        registerOutsideClose: function(container, dotnet, method) {
            if (!dotnet || !method) return;
            const handler = (ev) => {
                const t = ev.target;
                if (t && t.closest && t.closest('.quark-calendar-panel, .quark-date-container')) {
                    return;
                }
                dotnet.invokeMethodAsync(method).catch(()=>{});
            };
            document.addEventListener('mousedown', handler, true);
            return {
                dispose: function(){
                    document.removeEventListener('mousedown', handler, true);
                }
            };
        }
    };
})();
