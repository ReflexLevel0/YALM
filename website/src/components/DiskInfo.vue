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
      chartConfigDictionary: {}
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

      console.log("dic: ")
      console.log(this.$data.chartDataDictionary)
    },
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
  <VueCollapsiblePanelGroup>
    <VueCollapsiblePanel v-for="d in $data.disks">
      <template #title>{{d.model}}</template>
      <template #content>
        ===
        <div v-if="d.uuid in this.$data.chartDataDictionary">
          aaa
        </div>
        <Chart
          v-if="d.uuid in this.$data.chartDataDictionary"
          name="disk usage"
          :scales="{ x: { type: 'time' }, y: { min: 0, max: 100 } }"
          :chart-data="this.$data.chartDataDictionary[d.uuid]"
          @zoom-changed="async (limits) =>
          {
            $data.chartConfigDictionary[d.uuid].startDate = limits.startDate == null ? $props.startDate : limits.startDate;
            $data.chartConfigDictionary[d.uuid].endDate = limits.endDate == null ? $props.endDate : limits.endDate;
            await this.refreshAllData($data.chartConfigDictionary[d.uuid].startDate, $data.chartConfigDictionary[d.uuid].endDate)
          }"
        />
      </template>
    </VueCollapsiblePanel>
  </VueCollapsiblePanelGroup>
</template>

<style scoped>

</style>