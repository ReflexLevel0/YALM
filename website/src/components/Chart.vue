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
  emits: ['zoomChanged', 'reloadChart'],
  data() {
    return {
      noData: false,
      loadingData: false,
      loadedData: false,
      chartData: null,
      apiError: false,
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
    await this.refreshData()
  },
  methods: {
    async refreshData(){
      this.loadingData = true;
      this.getDataPromise.then((data) => {
        this.chartData = data;
        let dataCount = 0
        data.datasets.forEach(dataset => dataCount += dataset.data.length)
        this.$data.noData = dataCount === 0
        this.loadingData = false;
        this.loadedData = true;
      }).catch(() => {
        this.loadingData = false;
        this.loadedData = false;
        this.apiError = true;
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
    },
    reloadChart(){
      this.$emit('reloadChart')
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
      }
    }
  }
});
</script>

<template>
  <Scatter v-if="this.loadedData && this.noData === false" :data="chartData" :options="options" ref="chart" />
  <div v-if="this.loadingData" class="info">
    Loading {{ this.$props.name }} data...
  </div>
  <div v-if="this.noData" class="info">
    No data (try choosing a different date range)
  </div>
  <div v-if="this.apiError" class="info">
    API error! Try checking if API setup is correct.
  </div>
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
