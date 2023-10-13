<script lang="ts">
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
                enabled: true,
              },
              drag: {
                enabled: true,
                modifierKey: "ctrl",
              },
              pinch: {
                enabled: true,
              },
            },
          },
        },
      },
    };
  },
  async mounted() {
    this.loadingData = true;
    this.getDataPromise.then((data) => {
      this.chartData = data as never;
      this.loadingData = false;
      this.loadedData = true;
    });
  },
  props: {
    name: {
      type: String,
    },
    getDataPromise: {
      type: Promise,
      required: true,
    },
  },
});
</script>

<template>
  <Scatter v-if="this.loadedData" :data="chartData" :options="options" />
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
