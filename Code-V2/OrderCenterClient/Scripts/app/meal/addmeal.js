define(['ko', 'utility', 'jquery', 'app','validation'], function (ko, utility, $, app) {
    var self = {};
    self.name = ko.observable();
    self.price = ko.observable();
    self.isBreakfast = ko.observable();
    self.description = ko.observable();

    self.init = function () {
        applyValidation();
    }

    self.afterRender = function () {
        document.getElementById("price").onkeydown = function onlyNum() {
            var content = document.getElementById('price').value;
            var flag = content.indexOf('.') != -1;
            if (flag && event.keyCode == 110) {
                event.returnValue = false;
            }
            if (!(event.keyCode == 46) && !(event.keyCode == 8) && !(event.keyCode == 37) && !(event.keyCode == 39) && !(event.keyCode == 110))
                if (!((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105)))
                    event.returnValue = false;
        }
    }

    function applyValidation() {
        self.name.extend({
            required: {
                params: true,
                message: "请填写菜名"
            }
        }).isModified(false);

        self.price.extend({
            required: {
                params: true,
                message: "请填写价格"
            }
        }).isModified(false);
    }

    self.save = function () {
        self.errors = ko.validation.group(self);
        if (self.isValid()) {
            utility.baseAjax({
                url: '/Meal/AddMeal',
                type: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify({ "Name": self.name(), "Price": self.price(), "IsBreakfast": self.isBreakfast(), "Description": self.description() })
            }).done(function (data) {
                app.goTo("/meal");
            });
        }
        else {
            self.errors.showAllMessages();
        }
    };

    return self;
});