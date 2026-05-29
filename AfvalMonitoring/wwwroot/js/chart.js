window.renderLabelPieChart = (labels, values) => {
    const canvas = document.getElementById("labelPieChart");
    if (!canvas) {
        return;
    }

    if (window.labelPieChartInstance) {
        window.labelPieChartInstance.destroy();
    }

    const colors = [
        "#4e79a7",
        "#f28e2b",
        "#e15759",
        "#76b7b2",
        "#59a14f",
        "#edc948",
        "#b07aa1",
        "#ff9da7",
        "#9c755f",
        "#bab0ab"
    ];

    window.labelPieChartInstance = new Chart(canvas, {
        type: "pie",
        data: {
            labels: labels,
            datasets: [{
                data: values,
                backgroundColor: labels.map((_, i) => colors[i % colors.length])
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: "bottom"
                }
            }
        }
    });
};
