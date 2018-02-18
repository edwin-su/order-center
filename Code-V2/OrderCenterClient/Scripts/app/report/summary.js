define(['ko', 'utility', 'jquery', 'app', 'R'], function (ko, utility, $, app, R) {
    var self = {};

    self.orders = ko.observable();

    self.filterData = {
        affectDate: ko.observable(),
        mealTypeId: ko.observable(0),
        mealId: ko.observable(0),
        areaId: ko.observable(0),
        bedNumber: ko.observable('')
    };

    self.dates = ko.observable();
    self.meals = ko.observable();
    self.patientAreas = ko.observable();

    self.init = function () {
        initMealsAndAreas(self.filterData.affectDate());
        getFilterDays();
        //$.when(getFilterDays())
        //    .done(function () {
        //        getOrderList(ko.toJS(self.filterData));
        //    })
        //;
    };

    self.fitlerByDate = function (date) {
        if (!!date) {
            self.filterData.affectDate(date);
            getOrderList(ko.toJS(self.filterData));
            initMealsAndAreas(date);
        }
    };

    self.filterByMealType = function (mealTypeId) {
        if (!!mealTypeId) {
            self.filterData.mealTypeId(mealTypeId);
            getOrderList(ko.toJS(self.filterData));
        }
    };

    self.filterByPatientArea = function (areaId) {
        if (!!areaId) {
            self.filterData.areaId(areaId);

            getOrderList(ko.toJS(self.filterData));
        }
    }

    self.filterByMeal = function (mealId) {
        if (!!mealId) {
            self.filterData.mealId(mealId);
            getOrderList(ko.toJS(self.filterData));
        }
    }

    self.filterByBedNumber = function () {
        getOrderList(ko.toJS(self.filterData));
    }

    function initMealsAndAreas(date) {
        var deffered = $.Deferred();
        utility.baseAjax({
            url: '/Meal/GetMealsAndAreas',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ "mealDay": (new Date()).getDay() })
        })
            .done(function (data) {
                if (!!data) {
                    var lunchMealAndDinnerMeals = R.concat(data.LunchMeals, data.DinnerMeals);
                    var allMeals = R.concat(lunchMealAndDinnerMeals, data.BreakfastMeals);
                    var defaultMeals = R.concat([{ Name: '所有', Id: 0 }]);
                    var removeRepeatMeals = R.compose(R.dropRepeats, R.sortBy(R.prop('Id')));
                    self.meals(R.compose(removeRepeatMeals, defaultMeals)(allMeals));

                    var defaultArea = R.concat([{ AreaCode: '所有', Id: 0 }]);
                    self.patientAreas(defaultArea(data.PatientAreas));
                }
                deffered.resolve();
            })
            .fail(function () {
                deffered.reject();
            })
        return deffered;
    }

    function getFilterDays(filterData) {
        var deffered = $.Deferred();
        utility.baseAjax({
            url: '/Order/GetFilterDays',
            type: "GET",
            dataType: "json",
            data: filterData
        }).done(function (dates) {
            var list = [];
            self.filterData.affectDate(dates.Dates[0]);
            $.each(dates.Dates, function (key, value) {
                list.push( { Key: value, Value: value });
            })
            
            self.dates(list);
            deffered.resolve();
        }).fail(function () {
            deffered.reject();
        })
        return deffered;
    };

    function getOrderList(filterData) {
        utility.baseAjax({
            url: '/Order/GetOrders',
            type: "GET",
            dataType: "json",
            data: filterData
        }).done(function (orders) {
            self.orders(orders);
        });
    };

    //function getMealsAndArea() {
    //    utility.baseAjax({
    //        url: '/Meal/GetMealsAndAreas',
    //        type: "GET",
    //        dataType: "json",
    //        contentType: "application/json",
    //        data: JSON.stringify({ "mealDay": 1 })
    //    }).done(function (orders) {
    //        self.orders(orders);
    //    });
    //};

    return self;
});