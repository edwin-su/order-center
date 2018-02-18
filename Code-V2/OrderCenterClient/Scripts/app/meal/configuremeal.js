define(['ko', 'utility', 'jquery', 'app', 'R', 'validation'], function (ko, utility, $, app, R) {
    var self = {};

    self.mealDay = ko.observable((new Date()).getDay());

    self.meals = ko.observable();

    self.displayMeals = ko.observable();

    self.scheduleMeals = ko.observable();

    self.isSelectMeal = ko.observable(false);

    self.init = function () {
        getAllMeals();

        getScheduleMeals(self.mealDay());
    };

    self.selectDay = function (day) {
        self.mealDay(day);
        self.isSelectMeal(false);
        getScheduleMeals(day);
    }

    self.mealType;
    self.selectMeal = function (mealType) {
        self.isSelectMeal(true);

        self.mealType = mealType;

        self.displayMeals(filterMeals(self.meals(), self.scheduleMeals(), mealType, self.mealDay()));
    }

    self.configureMeal = function (meal) {
        utility.baseAjax({
            url: '/Meal/ConfigureMeal',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ "MealId": meal.Id, "MealType": self.mealType, "MealDay": self.mealDay() })
        }).done(function (meals) {
            getScheduleMeals(self.mealDay());

            meal.isAdded(true);
        });
    };

    self.deleteSchedule = function (id) {
        utility.baseAjax({
            url: '/Meal/DeleteSchedule',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ "id": id })
        }).done(function (meals) {
            getScheduleMeals(self.mealDay());
        });
    }

    self.back = function () {
        self.isSelectMeal(false);
    };

    function filterMeals(meals, scheduleMeals, mealType, mealDay) {
        if (!meals) {
            return [];
        }

        var result = R.map(function (meal) {
            if (R.find(R.whereEq({ "MealId": meal.Id, "MealDay": mealDay, "MealType": mealType }))(scheduleMeals)) {
                meal.isAdded = ko.observable(true);
            } else {
                meal.isAdded = ko.observable(false);
            }

            return meal;
        }, meals);

        return result;
    };

    function getAllMeals() {
        utility.baseAjax({
            url: '/Meal/GetMeals',
            type: "GET",
            dataType: "json",
        }).done(function (meals) {
            var removeBreakfast = R.filter(R.propEq("IsBreakfast", false));

            meals = removeBreakfast(meals);
            self.meals(meals);
        });
    }

    function getScheduleMeals(mealDay) {
        utility.baseAjax({
            url: '/Meal/GetSchedureMeals',
            type: "GET",
            dataType: "json",
            data: { "mealDay": mealDay }
        }).done(function (meals) {
            self.scheduleMeals(meals);
        });
    }

    return self;
});