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
      loadingData: false,
      loadedData: false,
      chartData: null,
      options: {
        responsive: true,
        maintainAspectRatio: true,
        scales: {
          x: {
            type: "time",
            time: {
              unit: "day",
            },
          },
          y: {
            min: 0,
          },
        },
        plugins: {
          zoom: {
            pan: {
              enabled: true,
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
                if(chart.chart.getZoomLevel() !== 1) this.emitZoomChanged(chart);
              },
            },
          },
        },
      },
    };
  },
  async mounted() {
    this.refreshData()
  },
  methods: {
    refreshData(){
      this.loadingData = true;
      this.getDataPromise.then((data) => {
        this.chartData = data;
        this.loadingData = false;
        this.loadedData = true;
      });
    },
    emitZoomChanged(chart){
      let chartJS = chart.chart
      const startDate = new Date(chartJS.scales.x.min)
      const endDate = new Date(chartJS.scales.x.max)
      this.$emit('zoomChanged', {
          startDate: startDate,
          endDate: endDate
      })
    }
  },
  props: {
    name: {
      type: String,
    },
    getDataPromise: {
      required: true,
    },
  },
  watch: {
    'getDataPromise': {
      handler: function(){
        this.refreshData()
        this.$refs.chart.chart.resetZoom()
      }
    }
  }
});
</script>

<template>
  <Scatter v-if="this.loadedData" :data="chartData" :options="options" ref="chart" />
  <div v-if="this.loadingData" class="loading-info">
    Loading {{ this.$props.name }} data...
  </div>
</template>

<style scoped>
.loading-info {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}
</style>
