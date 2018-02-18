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
                "paymentMethodType": data, "paymentMethodStatus": paymentMethodStatus})
        }).done(function (data) {
            if (!!data) {
                switch (data.PaymentMethodType) {
                    case self.paymentType.cash:
                        self.enableCash(data.PaymentMethodStatus);
                        break;
                    case self.paymentType.alipay:
                        self.enableAlipay(data.PaymentMethodStatus);
                        break;
                    case self.paymentType.wechat:
                        self.enableWechat(data.PaymentMethodStatus);
                        break;
                    case self.paymentType.prepaidcard:
                        self.enablePrepaidCard(data.PaymentMethodStatus);
                        break;
                }
            }
        });
    }

    return self;
});