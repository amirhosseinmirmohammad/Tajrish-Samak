$(function() {

    Morris.Line({
        element: 'morris-one-line-chart',
            data: [
                { year: '1393', value: 5 },
                { year: '1394', value: 10 },
                { year: '1395', value: 8 },
                { year: '1396', value: 22 },
                { year: '1397', value: 8 },
                { year: '1398', value: 10 },
                { year: '1399', value: 5 }
            ],
        xkey: 'year',
        ykeys: ['value'],
        resize: true,
        lineWidth:4,
        labels: ['Value'],
        lineColors: ['#1ab394'],
        pointSize:5,
    });

    Morris.Area({
        element: 'morris-area-chart',
        data: [{ period: '2010 Q1', iphone: 2666, ipad: null, itouch: 2647 },
            { period: '1393 Q2', iphone: 2778, ipad: 2294, itouch: 2441 },
            { period: '1393 Q3', iphone: 4912, ipad: 1969, itouch: 2501 },
            { period: '1393 Q4', iphone: 3767, ipad: 3597, itouch: 5689 },
            { period: '1394 Q1', iphone: 6810, ipad: 1914, itouch: 2293 },
            { period: '1394 Q2', iphone: 5670, ipad: 4293, itouch: 1881 },
            { period: '1394 Q3', iphone: 4820, ipad: 3795, itouch: 1588 },
            { period: '1394 Q4', iphone: 15073, ipad: 5967, itouch: 5175 },
            { period: '1394 Q1', iphone: 10687, ipad: 4460, itouch: 2028 },
            { period: '1394 Q2', iphone: 8432, ipad: 5713, itouch: 1791 } ],
        xkey: 'period',
        ykeys: ['iphone', 'ipad', 'itouch'],
        labels: ['iPhone', 'iPad', 'iPod Touch'],
        pointSize: 2,
        hideHover: 'auto',
        resize: true,
        lineColors: ['#87d6c6', '#54cdb4','#1ab394'],
        lineWidth:2,
        pointSize:1,
    });

    Morris.Donut({
        element: 'morris-donut-chart',
        data: [{ label: "Download Sales", value: 12 },
            { label: "In-Store Sales", value: 30 },
            { label: "Mail-Order Sales", value: 20 } ],
        resize: true,
        colors: ['#87d6c6', '#54cdb4','#1ab394'],
    });

    Morris.Bar({
        element: 'morris-bar-chart',
        data: [{ y: '1391', a: 60, b: 50 },
            { y: '1392', a: 75, b: 65 },
            { y: '1393', a: 50, b: 40 },
            { y: '1394', a: 75, b: 65 },
            { y: '1395', a: 50, b: 40 },
            { y: '1396', a: 75, b: 65 },
            { y: '1397', a: 100, b: 90 } ],
        xkey: 'y',
        ykeys: ['a', 'b'],
        labels: ['Series A', 'Series B'],
        hideHover: 'auto',
        resize: true,
        barColors: ['#1ab394', '#cacaca'],
    });

    Morris.Line({
        element: 'morris-line-chart',
        data: [{ y: '1390', a: 100, b: 90 },
            { y: '1391', a: 75, b: 65 },
            { y: '1392', a: 50, b: 40 },
            { y: '1393', a: 75, b: 65 },
            { y: '1394', a: 50, b: 40 },
            { y: '1395', a: 75, b: 65 },
            { y: '1396', a: 100, b: 90 } ],
        xkey: 'y',
        ykeys: ['a', 'b'],
        labels: ['Series A', 'Series B'],
        hideHover: 'auto',
        resize: true,
        lineColors: ['#54cdb4','#1ab394'],
    });

});
