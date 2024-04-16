<script>
import { Api } from "@/api";
import { Disk } from "@/models/Disk";
import Chart from "@/components/Chart.vue";
import { VueCollapsiblePanel, VueCollapsiblePanelGroup } from "@dafcoe/vue-collapsible-panel";
import "@dafcoe/vue-collapsible-panel/dist/vue-collapsible-panel.css";
import { ChartHelper } from "@/ChartHelper";

export default {
  computed: {
    Api() {
      return Api
    }
  },
  data() {
    return {
      disks: Disk,
      chartDataDictionary: {},
      chartConfigDictionary: {},
      chartDataConfig: {
        startDate: null,
        endDate: null
      },
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
  async created(){
    await this.refreshAllData(this.$props.startDate, this.$props.endDate)
  },
  methods: {
    async refreshAllData(startDate, endDate){
      let disks = await Api.getDisks(startDate, endDate)
      this.$data.disks = []
      disks.forEach(d => {
        let datasets = []
        this.$data.disks.push(d)
        d.partitions.forEach(p => {
          ChartHelper.PartitionLogsToDataset(p.logs).datasets.forEach(dataset => datasets.push(dataset))
        })
        this.$data.chartDataDictionary[d.uuid] = {
          datasets: [
            {
              showLine: true,
              label: "memory used %",
              borderColor: "#fa1818",
              backgroundColor: "#fa1818",
              data: [
                {
                  "x": "2024-04-16T14:36:00.000+02:00",
                  "y": 30
                }
              ]
            }
          ]
        }
      })
    },
  }
}
</script>

<template>
<!--          <Chart-->
<!--            v-if="$data.chartDataDictionary.length !== 0"-->
<!--            v-for="d in $data.chartDataDictionary.keys()"-->
<!--            name="disk usage"-->
<!--            :scales="{ x: { type: 'time' }, y: { min: 0, max: 100 } }"-->
<!--            :chart-data="$data.chartDataDictionary[d.uuid]"/>-->
<!--&lt;!&ndash;            @zoom-changed="async (limits) =>&ndash;&gt;-->
<!--&lt;!&ndash;            {&ndash;&gt;-->
<!--&lt;!&ndash;              $data.chartConfigDictionary[d.uuid].startDate = limits.startDate == null ? $props.startDate : limits.startDate;&ndash;&gt;-->
<!--&lt;!&ndash;              $data.chartConfigDictionary[d.uuid].endDate = limits.endDate == null ? $props.endDate : limits.endDate;&ndash;&gt;-->
<!--&lt;!&ndash;              await this.refreshDiskData($data.chartConfigDictionary[d.uuid].startDate, $data.chartConfigDictionary[d.uuid].endDate)&ndash;&gt;-->
<!--&lt;!&ndash;            }&ndash;&gt;-->

</template>

<style scoped>

</style>