function drawLineChart(chartId, data, noDataText, monthlySummary) {
    if (data && data.length === 0) {
        document.getElementById(chartId).innerHTML = noDataText;
        return;
}
    else document.getElementById(chartId).innerHTML = "";

    var root = am5.Root.new(chartId);
    var myTheme = am5.Theme.new(root);
    root._logo?.dispose();
    myTheme.rule("Label").setAll({
        fontSize: "8px",
        fill: am5.color('#FFFFFF')
    });

    root.setThemes([
        am5themes_Animated.new(root),
        myTheme
    ]);

    let chart = root.container.children.push(
        am5xy.XYChart.new(root, {
            layout: root.verticalLayout,
            pinchZoomX: true
        })
    );

    let xRenderer = am5xy.AxisRendererX.new(root, {});
    xRenderer.grid.template.set("location", 0.5);
    xRenderer.labels.template.setAll({
        location: 0.5,
        multiLocation: 0.5
    });

    let xAxis = chart.xAxes.push(
        am5xy.CategoryAxis.new(root, {
            categoryField: "month",
            renderer: xRenderer,
            tooltip: am5.Tooltip.new(root, {})
        })
    );

    xAxis.data.setAll(data);

    let yAxis = chart.yAxes.push(
        am5xy.ValueAxis.new(root, {
            maxPrecision: 0,
            renderer: am5xy.AxisRendererY.new(root, {
                // inversed: true
            })
        })
    );
    chart.set("colors", am5.ColorSet.new(root, {
        colors: [
            am5.color(0x73556E),
            am5.color(0x9FA1A6),
            am5.color(0xF2AA6B),
            am5.color(0xF28F6B),
            am5.color(0xA95A52),
            am5.color(0xE35B5D),
            am5.color(0xFFA446)
        ]
    }))

    var cursor = chart.set("cursor", am5xy.XYCursor.new(root, {
       // alwaysShow: true,
        xAxis: xAxis,
        positionX: 1
    }));

    cursor.lineY.set("visible", false);
    cursor.lineX.set("focusable", true);
    var legend = chart.children.push(am5.Legend.new(root, {
        centerX: am5.percent(50),
        x: am5.percent(50),
        layout: am5.GridLayout.new(root, {
            maxColumns: 10,
            fixedWidthGrid: true
        })
    }));
    legend.markers.template.setAll({
        width: 10,
        height: 10
    });
    legend.markerRectangles.template.adapters.add("fillGradient", function () {
        return undefined;
    })
    legend.markerRectangles.template.setAll({
        cornerRadiusTL: 5,
        cornerRadiusTR: 5,
        cornerRadiusBL: 5,
        cornerRadiusBR: 5
    });
    function createSeries(name, field,color) {
        let series = chart.series.push(
            am5xy.LineSeries.new(root, {
                name: name,
                maskBullets:false,
                fill: am5.color(color),
                stroke: am5.color(color),
                xAxis: xAxis,
                yAxis: yAxis,
                valueYField: field,
                categoryXField: "month",
                tooltip: am5.Tooltip.new(root, {
                    pointerOrientation: "horizontal",
                    labelText: "[bold]{name}[/]\n{categoryX}: {valueY}"
                })
            })
        );

        series.bullets.push(function () {
            return am5.Bullet.new(root, {
                sprite: am5.Circle.new(root, {
                    fill: series.get("fill")
                })
            });
        });
        series.strokes.template.setAll({
            strokeWidth: 1
        });

        series.set("setStateOnChildren", true);
        series.states.create("hover", {});

        series.mainContainer.set("setStateOnChildren", true);
        series.mainContainer.states.create("hover", {});

        series.strokes.template.states.create("hover", {
            strokeWidth: 1
        });

        legend.data.setAll(chart.series.values);
        series.data.setAll(data);
        series.appear(1000);
    }
    if (monthlySummary) {
        createSeries("Income", "income", "#008000");//Green
        createSeries("Expense", "expense", "#ff0000"); // Red
        createSeries("Saving", "saving", "#4b9eb9");//Green
        createSeries("FixedLiabilities", "fixedLiabilities", "#5644a6"); // Red
        createSeries("AmountReceivable", "amountReceivable", "#914916");//Green
        createSeries("AmountPayable", "amountPayable", "#ff00ff"); // Red
        createSeries("SpiritualSpends", "spiritualSpends", "#77da77");//Green
        createSeries("CreditSpend", "creditSpend", "#f22626"); // Red
        createSeries("CreditOutstanding", "creditOutstanding", "#f22626");//Green
        createSeries("Investment", "investment", "#ff00ff"); // Red
        createSeries("Returns", "returns", "#00ff00");//Green
    }
    else {
        createSeries("Income", "income", "#008000");//Green
        createSeries("Expense", "expense", "#ff0000"); // Red
    }

    chart.plotContainer.events.on("pointerout", function () {
        cursor.set("positionX", 1)
    })
    chart.plotContainer.events.on("pointerover", function () {
        cursor.set("positionX", undefined)
    })
    chart.appear(1000, 100);
}