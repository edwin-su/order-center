define(['ko', 'utility', 'jquery', 'app', 'R'], function (ko, utility, $, app, R) {
    var self = {};

    self.meals = ko.observable();

    self.init = function () {
        //applyValidation();
        getAllMeals();
    }

    self.name = ko.observable();
    self.price = ko.observable();
    self.isBreakfast = ko.observable();
    self.description = ko.observable();

    //self.init = function () {
        
    //}

    self.afterRender = function () {
        //document.getElementById("price").onkeydown = function onlyNum() {
        //    var content = document.getElementById('price').value;
        //    var flag = content.indexOf('.') != -1;
        //    if (flag && event.keyCode == 110) {
        //        event.returnValue = false;
        //    }
        //    if (!(event.keyCode == 46) && !(event.keyCode == 8) && !(event.keyCode == 37) && !(event.keyCode == 39) && !(event.keyCode == 110))
        //        if (!((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105)))
        //            event.returnValue = false;
        //}
    }

    //function applyValidation() {
    //    self.name.extend({
    //        required: {
    //            params: true,
    //            message: "请填写菜名"
    //        }
    //    }).isModified(false);

    //    self.price.extend({
    //        required: {
    //            params: true,
    //            message: "请填写价格"
    //        }
    //    }).isModified(false);
    //}

    self.save = function () {
        //self.errors = ko.validation.group(self);
        if (!self.name()) {
            alert("菜名不能为空");
            return;
        } else if (!self.price() || isNaN(self.price()) || self.price() < 0) {
            alert("菜价不符合要求");
            return;
        } else if (!self.description()) {
            alert("请对菜品做简要描述");
            return;
        }

        utility.baseAjax({
            url: '/Meal/AddMeal',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ "Name": self.name(), "Price": self.price(), "IsBreakfast": self.isBreakfast(), "Description": self.description() })
        }).done(function (data) {
            self.name(null);
            self.price(null);
            self.isBreakfast(false);
            self.description(null);
            getAllMeals();
        });
    };




    self.delete = function (id) {
        var meal = R.find(R.propEq('Id', id))(self.meals());
        var result = confirm("你将会删除：" + meal.Name)
        if (result) {
            deleteMeal(id);
        }
    }

    function getAllMeals() {
        utility.baseAjax({
            url: '/Meal/GetMeals',
            type: "GET",
            dataType: "json",
        }).done(function (meals) {
            self.meals(meals);
        });
    }

    function deleteMeal(id) {
        utility.baseAjax({
            url: '/Meal/DeleteMeal',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ "id": id })
        }).done(function () {
            deleteMealFromArray(id)
        });
    }

    function deleteMealFromArray(id) {
        var meals = self.meals();
        var mealIndex = R.findIndex(R.propEq('Id', id))(meals);
        meals = R.remove(mealIndex, 1, meals);
        self.meals(meals);
    }

    return self;
});