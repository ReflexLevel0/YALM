<script>
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  TimeScale,
} from "chart.js";
import { Scatter } from "vue-chartjs";
import "chartjs-adapter-date-fns";
import zoomPlugin from "chartjs-plugin-zoom";
import { defineComponent } from "vue";

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  TimeScale,
  zoomPlugin
);

export default defineComponent({
  name: "App",
  components: {
    Scatter,
  },
  emits: ['zoomChanged'],
  data() {
    return {
      loadingData: true,
      noData: false,
      chart: null,
      options: {
        responsive: true,
        maintainAspectRatio: true,
        scales: {
          x: {
            type: "time",
          },
          y: {
            min: 0,
          },
        },
        plugins: {
          zoom: {
            pan: {
              enabled: false,
              modifierKey: "ctrl",
              mode: "x",
            },
            scales: {
              y: {
                min: 0,
                max: 100,
              },
            },
            zoom: {
              mode: "x",
              wheel: {
                enabled: false,
              },
              drag: {
                enabled: true,
                modifierKey: "alt",
              },
              pinch: {
                enabled: false,
              },
              onZoomComplete: (chart) => {
                if(chart.chart.getZoomLevel() !== 1) {
                  this.emitZoomChanged(chart);
                }
              },
            },
          },
        },
      },
    };
  },
  async mounted() {
    this.$data.options.scales = this.$props.scales
  },
  methods: {
    emitZoomChanged(chart){
      let chartJS = chart?.chart
      const startDate = chartJS == null ? null : new Date(chartJS.scales.x.min)
      const endDate = chartJS == null ? null : new Date(chartJS.scales.x.max)
      this.$emit('zoomChanged', {
          startDate: startDate,
          endDate: endDate
      })
    },
    reloadChart(){
      this.$data.loadingData = true
      this.$emit('zoomChanged', {
        startDate: null,
        endDate: null
      })
    }
  },
  props: {
    name: {
      type: String,
    },
    chartData: {
      type: Object
    },
    scales: {
      type: Object,
      required: true
    },
  },
  watch: {
    "chartData": function(){
      this.$data.loadingData = false
      this.$data.noData = this.$props.chartData.data === 0
    }
  }
});
</script>

<template>
  <div v-if="this.loadingData" class="info">
    Loading {{ this.$props.name }} data...
  </div>
  <div v-else-if="this.noData" class="info">
    No data (try choosing a different date range)
  </div>
  <Scatter v-else-if="this.loadingData === false" :id="chart" :data="this.$props.chartData" :options="options" />
  <button @click="reloadChart">Reset zoom</button>
</template>

<style scoped>
.info {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}
</style>