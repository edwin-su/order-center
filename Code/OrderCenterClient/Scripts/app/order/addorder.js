define(['ko', 'utility', 'jquery'], function (ko, utility, $) {
    var self = {};

    self.isToday = ko.observable(false);
    self.mealInfo = ko.observable("");
    self.breakfast = ko.observable("");
    self.lunchMealId = ko.observable("");
    self.lunchMealName = ko.observable("");
    self.dinnerMealId = ko.observable("");
    self.dinnerMealName = ko.observable("");

    self.lunchMeals = ko.observableArray([]);
    self.dinnerMeals = ko.observableArray([]);
    self.areas = ko.observableArray([]);
    self.selectedOption = ko.observable();
    self.whenchage = function (p) {
        var a = p.selectedOption();
    };  

    function resetSelector() {
        self.mealInfo("");
        self.breakfast("");
        self.lunchMealId("");
        self.dinnerMealId("");
    }

    function getMealList(isToday) {
        var deffered = $.Deferred();
        utility.baseAjax({
            url: '/OrderMeal/GetMealToOrder',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ "isToday": isToday })
        })
        .done(function (data) {
            deffered.resolve(data);
        })
        .fail(function () {
            deffered.reject();
        })
        return deffered;
    }

    function init() {
        resetSelector();
        $.when(getMealList(false))
        .done(function (data) {
            if (!!data) {
                var lunchList = [];
                var dinnerList = [];
                for (var i = 0; i < data.Meals.length; i++) {
                    var meal = ko.mapping.fromJS(data.Meals[i]);
                    if (i < 3) {
                        lunchList.push(meal);
                    } else {
                        dinnerList.push(meal);
                    }
                }
                self.lunchMeals(lunchList);
                self.dinnerMeals(dinnerList);

                var areas = [];
                $.each(data.Areas, function (key, value) {
                    var area = ko.mapping.fromJS(value);
                    areas.push(area);
                })

                self.areas(areas);
            }
        });
    }

    self.init = function () {
        init();
    }

    return self;
});