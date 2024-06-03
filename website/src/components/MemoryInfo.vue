<script>
import { Memory } from "@/models/Memory";
import Chart from "@/components/Chart.vue"
import { Api } from "@/api";
import { ChartHelper } from "@/ChartHelper";
import Fieldset from "primevue/fieldset";

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
    endDate: null,
    serverId: {
      required: true
    }
  },
  components: {
    Chart,
    Fieldset
  },
  methods: {
    async refreshData(startDate = this.$props.startDate, endDate = this.$props.endDate) {
      this.$data.memory = await Api.getMemory(this.$props.serverId, startDate, endDate)
      this.$data.memoryChartData = ChartHelper.MemoryLogsToDataset(this.$data.memory?.logs)
    }
  },
  async mounted() {
    await this.refreshData()
  },
  watch: {
    "startDate": async function() {
      await this.refreshData();
    },
    "endDate": async function() {
      await this.refreshData();
    },
    "serverId": async function() {
      await this.refreshData()
    }
  }
}
</script>

<template>
  <Fieldset legend="Memory" :toggleable="true">
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
  </Fieldset>
</template>