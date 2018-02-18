define(['ko', 'utility', 'jquery'], function (ko, utility, $) {
    var self = {};

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

    self.togglePaymentMethod = function (data, element) {
        var paymentMethodStatus = false;
        switch (data) {
            case self.paymentType.cash:
                paymentMethodStatus = self.enableCash();
                break;
            case self.paymentType.alipay:
                paymentMethodStatus = self.enableAlipay();
                break;
            case self.paymentType.wechat:
                paymentMethodStatus = self.enableWechat();
                break;
            case self.paymentType.prepaidcard:
                paymentMethodStatus = self.enablePrepaidCard();
                break;
        }
        utility.baseAjax({
            url: '/PaymentMethod/TogglePaymentMethodStatus',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({
                "paymentMethodTypeId": data, "paymentMethodStatus": paymentMethodStatus})
        }).done(function (result) {
            if (!!result && result.UpdatePaymentMethodStatusScussfully) {
                switch (data) {
                    case self.paymentType.cash:
                        self.enableCash(!paymentMethodStatus);
                        break;
                    case self.paymentType.alipay:
                        self.enableAlipay(!paymentMethodStatus);
                        break;
                    case self.paymentType.wechat:
                        self.enableWechat(!paymentMethodStatus);
                        break;
                    case self.paymentType.prepaidcard:
                        self.enablePrepaidCard(!paymentMethodStatus);
                        break;
                }
            }
        });
    }

    function init() {
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
            }
        });
    }

    self.init = function () {
        init();
    }

    return self;
});