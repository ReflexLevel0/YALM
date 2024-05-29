<script>
import { Api } from "@/api";
import { Disk } from "@/models/Disk";
import Chart from "@/components/Chart.vue";
import { VueCollapsiblePanel, VueCollapsiblePanelGroup } from "@dafcoe/vue-collapsible-panel";
import { ChartHelper } from "@/ChartHelper";

export default {
  computed: {
    Api() {
      return Api
    }
  },
  data() {
    return {
      disks: [],
      chartDataDictionary: {},
      chartConfigDictionary: {},
      chartData: null
    }
  },
  props: {
    startDate: null,
    endDate: null
  },
  components: {
    Chart,
    VueCollapsiblePanel,
    VueCollapsiblePanelGroup
  },
  async mounted(){
    await this.refreshAllData(this.$props.startDate, this.$props.endDate)
  },
  methods: {
    async refreshAllData(startDate, endDate){
      let disks = await Api.getDisks(startDate, endDate)
      this.$data.disks = []
      disks.forEach(d => {
        this.$data.disks.push(d)
        this.$data.chartDataDictionary[d.uuid] = {datasets: []}
        this.$data.chartConfigDictionary[d.uuid] = {
          startDate: this.$props.startDate,
          endDate: this.$props.endDate
        }
        d.partitions.forEach(p => {
          ChartHelper.PartitionLogsToDataset(p.logs).datasets.forEach(dataset => this.$data.chartDataDictionary[d.uuid].datasets.push(dataset))
        })
      })

      console.log("disks: ")
      console.log(this.$data.disks)
      console.log("chart data dictionary: ")
      console.log(this.$data.chartDataDictionary)
    },
    async refreshDiskData(startDate, endDate){

    }
  },
  watch: {
    "startDate": async function() {
      await this.refreshAllData(this.$props.startDate, this.$props.endDate);
    },
    "endDate": async function() {
      await this.refreshAllData(this.$props.startDate, this.$props.endDate);
    }
  }
}
</script>

<template>
  <VueCollapsiblePanel v-for="d in disks">
    <template #title>{{d.uuid}}</template>
    <template #content>
      ===
      <Chart
        name="Disk usage"
        :scales="{ x: { type: 'time' }, y: { min: 0, max: 100 } }"
        :chart-data="this.$data.chartDataDictionary[d.uuid]"
        @zoom-changed="async (limits) =>
        {
          $data.chartConfigDictionary[d.uuid].startDate = limits.startDate == null ? $props.startDate : limits.startDate;
          $data.chartConfigDictionary[d.uuid].endDate = limits.endDate == null ? $props.endDate : limits.endDate;
          await this.refreshDiskData(d)
        }"
      />
    </template>
  </VueCollapsiblePanel>
</template>

<style scoped>

</style>