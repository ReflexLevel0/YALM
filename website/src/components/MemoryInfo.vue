<script>
import { Memory } from "@/models/Memory";
import Chart from "@/components/Chart.vue"
import { Api } from "@/api";
import { ChartHelper } from "@/ChartHelper";

export default {
  data(){
    return {
      memory: Memory,
      memoryChartData: null,
      memoryChartConfig: {
        startDate: null,
        endDate: null
      }
    }
  },
  props: {
    startDate: null,
    endDate: null
  },
  components: {
    Chart
  },
  methods: {
    async refreshData(startDate, endDate) {
      this.$data.memory = await Api.getMemory(startDate, endDate)
      this.$data.memoryChartData = ChartHelper.MemoryLogsToDataset(this.$data.memory.logs)
    }
  },
  async mounted() {
    await this.refreshData(this.$props.startDate, this.$props.endDate)
  },
  watch: {
    "startDate": async function() {
      await this.refreshData(this.$props.startDate, this.$props.endDate);
    },
    "endDate": async function() {
      await this.refreshData(this.$props.startDate, this.$props.endDate);
    }
  }
}
</script>

<template>
  <div>
    <Chart
      name="Memory"
      :scales="{ x: { type: 'time' }, y: { min: 0, max: 100 }}"
      :chart-data="this.$data.memoryChartData"
      @zoom-changed="async (limits) => {
      $data.memoryChartConfig.startDate = limits.startDate ?? $props.startDate
      $data.memoryChartConfig.endDate = limits.endDate ?? $props.endDate
      await this.refreshData($data.memoryChartConfig.startDate, $data.memoryChartConfig.endDate)
    }"
    />
  </div>
</template>