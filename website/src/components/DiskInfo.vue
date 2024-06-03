<script>
import { Api } from "@/api";
import Chart from "@/components/Chart.vue";
import { ChartHelper } from "@/ChartHelper";
import Fieldset from "primevue/fieldset";

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
    endDate: null,
    serverId: {
      required: true
    }
  },
  components: {
    Chart,
    Fieldset
  },
  async mounted(){
    await this.refreshAllData(this.$props.startDate, this.$props.endDate)
  },
  methods: {
    async refreshAllData(startDate = this.$props.startDate, endDate = this.$props.endDate){
      let disks = await Api.getDisks(this.$props.serverId, startDate, endDate)
      this.$data.disks = []
      disks.forEach(d => {
        this.$data.disks.push(d)
        this.$data.chartDataDictionary[d.uuid] = {datasets: this.partitionsToDatasets(d)}
        this.$data.chartConfigDictionary[d.uuid] = this.getChartConfig(startDate, endDate)
      })
      console.log("disks: ")
      console.log(this.$data.disks)
      console.log("chart data dictionary: ")
      console.log(this.$data.chartDataDictionary)
    },

    async refreshDiskData(disk){
      let config = this.$data.chartConfigDictionary[disk.uuid]
      let newDisk = await Api.getDisk(this.$props.serverId, config.startDate, config.endDate, disk.uuid)
      if (newDisk == null) {
        console.log("ERROR IN FETCHING DISK DATA FOR DISK WITH UUID " + disk.uuid)
        return
      }

      this.$data.chartDataDictionary[newDisk.uuid] = {datasets: this.partitionsToDatasets(newDisk)}
      this.$data.chartConfigDictionary[newDisk.uuid] = this.getChartConfig(config.startDate, config.endDate)
    },

    partitionsToDatasets(disk){
      let initialColors = ["#1a881c", "#8e3cc2", "#00e6ff", "#ffcb5a", "#ff00e9", "#5d311a", "#62811a"]
      let colors = []
      let datasets = []

      disk.partitions.forEach(p => {
        if(colors.length === 0){
          initialColors.forEach(c => colors.push(c))
        }

        //each dataset has a random color (unless that color has already been used for this disk's partitions)
        let randomColorIndex = Math.floor(colors.length * Math.random())
        let randomColor = colors.splice(randomColorIndex, 1)[0]
        datasets.push(ChartHelper.PartitionLogsToDataset(p.label, randomColor, p.logs))
      })
      return datasets
    },

    getChartConfig(startDate, endDate){
      return {
        startDate: startDate,
          endDate: endDate
      }
    }
  },
  watch: {
    "startDate": async function() {
      await this.refreshAllData();
    },
    "endDate": async function() {
      await this.refreshAllData();
    },
    "serverId": async function() {
      await this.refreshAllData()
    }
  }
}
</script>

<template>
  <Fieldset legend="Disks" :toggleable="true">
    <Fieldset v-for="d in disks" :legend="d.uuid" :toggleable="true">
      <Chart
        name="Disk usage"
        :scales="{ x: { type: 'time' }, y: { min: 0, max: 100 } }"
        :chart-data="this.$data.chartDataDictionary[d.uuid]"
        @zoom-changed="async (limits) =>
          {
            $data.chartConfigDictionary[d.uuid].startDate = limits.startDate == null ? $props.startDate : limits.startDate
            $data.chartConfigDictionary[d.uuid].endDate = limits.endDate == null ? $props.endDate : limits.endDate
            await this.refreshDiskData(d)
          }"
      />
    </Fieldset>
  </Fieldset>
</template>

<style scoped>

</style>