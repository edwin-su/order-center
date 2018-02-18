define(['ko', 'utility', 'jquery', 'app','chart','utils'], function (ko, utility, $, app) {
    var self = {};

    //var MONTHS = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    

    self.afterRender = function () {
        var ctx = document.getElementById("canvas").getContext("2d");
        utility.baseAjax({
            url: '/Order/GetSalesReport',
            type: "GET",
        })
        .done(function (data) {
            if (!!data && !!data.SalesData && data.SalesData.length > 0) {
                var dates = [];
                var sales = [];
                $.each(data.SalesData, function (key, value) {
                    dates.push(value.OrderAffectDay);
                    sales.push(value.SalesCount);
                });
                var config = {
                    type: 'line',
                    data: {
                        labels: dates,
                        datasets: [{
                            label: "销售量",
                            backgroundColor: window.chartColors.red,
                            borderColor: window.chartColors.red,
                            data: sales,
                            fill: false,
                        }]
                    },
                    options: {
                        responsive: true,
                        title: {
                            display: true,
                            text: '每日销量统计'
                        },
                        tooltips: {
                            mode: 'index',
                            intersect: false,
                        },
                        hover: {
                            mode: 'nearest',
                            intersect: true
                        },
                        scales: {
                            xAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: '销售日期'
                                }
                            }],
                            yAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: '销售份数'
                                }
                            }]
                        }
                    }
                };
                window.myLine = new Chart(ctx, config);
            }
        })
        .fail(function () {

        })

        
    }
    
    return self;
});