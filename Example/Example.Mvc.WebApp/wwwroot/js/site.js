(function (listhelper, $, undefined) {
    //Private Property
    var targetContainer = ".content-box";
    var listType = "ol"

    //Public Property
    listhelper.containerobject = $(targetContainer);
    listhelper.listobject = null;

    //Public Method
    listhelper.append = function (text) {

        if (listhelper.containerobject.children(listType).length == 0) {
            listhelper.containerobject.text("");
            listhelper.containerobject.append("<" + listType + ">");
        }

        if (listhelper.listobject == undefined) {
            listhelper.listobject = listhelper.containerobject.children(listType);
        }

        addItem(text);
    };

    listhelper.clear = function (text) {
        if (listhelper.listobject !== undefined) {
            listhelper.containerobject.text("None");
            listhelper.listobject = null;
        }
    }

    //Private Method
    function addItem(item) {
        if (item !== undefined) {
            listhelper.listobject.append($("<li>").text(item));
        }
    }
}(window.listhelper = window.listhelper || {}, jQuery));