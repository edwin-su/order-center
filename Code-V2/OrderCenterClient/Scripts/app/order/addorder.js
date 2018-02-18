define(['ko', 'utility', 'jquery'], function (ko, utility, $) {
    var self = {};

    self.bedNumber = ko.observable("");
    self.selectedBreakfastMeals = ko.observableArray([]);
    self.lunchMeal = ko.observable();
    self.dinnerMeal = ko.observable();

    self.breakfastMeals = ko.observableArray([]);
    self.lunchMeals = ko.observableArray([]);
    self.dinnerMeals = ko.observableArray([]);
    self.patientAreas = ko.observableArray([]);
    self.selectedOption = ko.observable();
    self.selectedPatientArea = ko.observable();
    self.canBePaied = ko.observable(false);
    self.amountToPay = ko.observable(0);
    self.paymentMethodId = ko.observable();
    self.enableCash = ko.observable(false);
    self.enableAlipay = ko.observable(false);
    self.enableWechat = ko.observable(false);
    self.enablePrepaidCard = ko.observable(false);

    self.paymentType = {
        cash: 1,
        alipay: 2,
        wechat: 3,
        prepaidcard: 4
    }

    self.changePatientArea = function (p) {
        if (!!p.selectedOption()) {
            self.selectedPatientArea(p.selectedOption()[0]);
        }
    };  

    function resetSelector() {
        self.bedNumber("");
        self.selectedBreakfastMeals([]);
        self.breakfastMeals([]);
        self.lunchMeal("");
        self.dinnerMeal("");
        self.paymentMethodId(0);
        self.canBePaied(false);
    }

    function getMealList(isToday) {
        var deffered = $.Deferred();
        utility.baseAjax({
            url: '/Meal/GetMealsAndAreas',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ "mealDay": ((new Date()).getDay() + 1) })
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
                var breakfastList = [];
                for (var i = 0; i < data.LunchMeals.length; i++) {
                    var meal = ko.mapping.fromJS(data.LunchMeals[i]);
                    lunchList.push(meal);
                }
                for (var i = 0; i < data.DinnerMeals.length; i++) {
                    var meal = ko.mapping.fromJS(data.DinnerMeals[i]);
                    dinnerList.push(meal);
                }
                for (var i = 0; i < data.BreakfastMeals.length; i++) {
                    var meal = ko.mapping.fromJS(data.BreakfastMeals[i]);
                    breakfastList.push(meal);
                }
                self.lunchMeals(lunchList);
                self.dinnerMeals(dinnerList);
                self.breakfastMeals(breakfastList);

                var patientAreas = [];
                $.each(data.PatientAreas, function (key, value) {
                    var area = ko.mapping.fromJS(value);
                    patientAreas.push(area);
                })

                self.patientAreas(patientAreas);
            }
        });
    }

    self.goToSelectingPaymentMethod = function () {
        if (!self.bedNumber() || !self.selectedPatientArea()) {
            alert("床位号和病区不能为空");
        } else {
            if (self.canBePaied()) {
                self.canBePaied(false);
                self.paymentMethodId(null);
            } else {
                utility.baseAjax({
                    url: '/PaymentMethod/GetPaymentMethodStatus',
                    type: "GET",
                    dataType: "json",
                }).done(function (data) {
                    if (!!data && !!data.PaymentMethods) {
                        $.each(data.PaymentMethods, function (key, value) {
                            if (value.PaymentMethodTypeId == self.paymentType.cash) {
                                self.enableCash(value.IsActive);
                            }
                            if (value.PaymentMethodTypeId == self.paymentType.alipay) {
                                self.enableAlipay(value.IsActive);
                            }
                            if (value.PaymentMethodTypeId == self.paymentType.wechat) {
                                self.enableWechat(value.IsActive);
                            }
                            if (value.PaymentMethodTypeId == self.paymentType.prepaidcard) {
                                self.enablePrepaidCard(value.IsActive);
                            }
                        })
                        self.canBePaied(true);
                        calculate();
                    }
                });
                
            }
            
        }
    }

    self.choosePaymentMethod = function (data) {
        if (!!data && data > 0) {
            self.paymentMethodId(data);
        }
    }

    function calculate() {
        if (!self.selectedBreakfastMeals() && self.selectedBreakfastMeals().length == 0 && !self.lunchMeal() && !self.dinnerMeal()) {
            alert("未选择餐食");
        } else {
            var breakfastPrice = 0;
            $.each(self.selectedBreakfastMeals(), function (key, value) {
                breakfastPrice += value.Price();
            });
            var lunchPrice = !!self.lunchMeal() ? self.lunchMeal().Price() : 0;
            var dinnerPrice = !!self.dinnerMeal() ? self.dinnerMeal().Price() : 0;
            self.amountToPay(breakfastPrice + lunchPrice + dinnerPrice);
        }
    }

    self.addToLunch = function (data) {
        if (!!data) {
            if (data.IsSelected()) {
                data.IsSelected(false);
                self.lunchMeal(null);
            } else {
                data.IsSelected(true);
                self.lunchMeal(data);
            }
        }
    }

    self.addToDinner = function (data) {
        if (!!data) {
            if (data.IsSelected()) {
                data.IsSelected(false);
                self.dinnerMeal(null);
            } else {
                data.IsSelected(true);
                self.dinnerMeal(data);
            }
        }
    }

    self.addToBreakfast = function (data) {
        if (!!data) {
            if (data.IsSelected()) {
                data.IsSelected(false);
                var list = self.selectedBreakfastMeals();
                list.splice(list.indexOf(list.filter(function (x) {
                    return x.IsSelected();
                })), 1);
                self.selectedBreakfastMeals(list);
            } else {
                data.IsSelected(true);
                var list = self.selectedBreakfastMeals();
                list.push(data);
                self.selectedBreakfastMeals(list);
            }
        }
    }

    self.generateMealOrder = function () {
        var mealOrder = {};
        mealOrder.Breakfasts = !!self.selectedBreakfastMeals() ? ko.mapping.toJS(self.selectedBreakfastMeals()) : null;
        mealOrder.Lunch = !!self.lunchMeal() ? ko.mapping.toJS(self.lunchMeal()) : null;
        mealOrder.Dinner = !!self.dinnerMeal() ? ko.mapping.toJS(self.dinnerMeal()) : null;
        mealOrder.BedNumber = self.bedNumber();
        mealOrder.PatientAreaId = self.selectedPatientArea();
        mealOrder.DeviceTypeId = getDeviceTypeId();
        mealOrder.PaymentMethodId = self.paymentMethodId();

        utility.baseAjax({
            url: '/Order/GenerateMealOrder',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ "mealOrder": mealOrder })
        })
        .done(function (data) {
            alert("下单成功，订单号：" + data.ReceiptNumber);
            location.href = '/addorder';
        })
        .fail(function () {
           
        })
    }

    function getDeviceTypeId() {
        var deviceType = 1;
        if (utility.isPC()) {
            deviceTypeId = 2;
        } else if (utility.isiPhone()) {
            deviceTypeId = 3;
        } else if (utility.isIpad()) {
            deviceTypeId = 4;
        } else if (utility.isAndroid()) {
            deviceTypeId = 5;
        }

        return deviceType;
    }

    self.init = function () {
        init();
    }

    return self;
});