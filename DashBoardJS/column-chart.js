Highcharts.chart('columnchart', {
    chart: {
        type: 'column',
         options3d: {
            enabled: true,
            alpha: 0,
            beta: 0,
            viewDistance:85,
            depth: 50
        }
    },

 
    xAxis: {
        categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May','Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
      
        
    },

    yAxis: {
        allowDecimals: false,
        min: 1,
        title: {
            text: 'Amount in Lacks'
        }
    },


    tooltip: {
        headerFormat: '<b>{point.key}</b><br>',
        pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y}'
    },

    plotOptions: {
        column: {
            stacking: 'normal',
            depth: 10
        }
    },

    series: [{
        name: 'Purchase',
        data: [5, 3, 4, 7, 2, 5, 3, 4, 7, 2, 5, 3],
        stack: 'male'
    }, {
        name: 'Issue',
         data: [3, 0, 4, 9, 3, 3, 0, 4, 3, 3, 0, 4],
        stack: 'female'
    }]
});