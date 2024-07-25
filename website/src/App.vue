<script>
import { Api } from "@/api";
import Menubar from "primevue/menubar";
import MegaMenu from "primevue/megamenu";
import {PrimeIcons} from "primevue/api";

export default {
  name: "app",
  data(){
    return{
      menu_items: [
        {
          label: 'Servers',
          icon: PrimeIcons.SERVER,
          items: []
        },
        {
          label: 'Alerts',
          icon: PrimeIcons.BELL,
          route: '/alerts'
        }
      ]
    }
  },
  components: {
    Menubar,
    MegaMenu
  },
  async created() {
    //fetching the list of servers and adding them to the "Server" dropdown when app gets created
    let servers = await Api.getServers()
    let first_server = undefined
    servers.forEach(s => {
      let serv = {label: s.serverId, icon: PrimeIcons.SERVER, route: '/' + s.serverId}
      if (first_server === undefined) first_server = serv
      this.$data.menu_items[0].items.push(serv)
    })

    // navigating to first server in the list when home page gets opened (if any servers even exist)
    if (first_server !== undefined) this.$router.push(first_server.route)
  }
};
</script>

<template>
  <nav style="margin-bottom: 20px">
    <div class="card">

      <!--main manu-->
      <Menubar :model="menu_items">
        <template #item="{item, props, hasSubmenu}">

          <!--creating a router link in case the item has a route-->
          <router-link v-if="item.route" v-slot="{ href, navigate }" :to="item.route" custom>
            <a :href="href" v-bind="props.action" @click="navigate">
              <span :class="item.icon" style="margin-right: 10px;" />
              <span class="ml-2">{{ item.label }}</span>
            </a>
          </router-link>

          <!--creating a normal item in case the item has no route-->
          <a v-else :href="item.url" :target="item.target" v-bind="props.action">
            <span :class="item.icon" style="margin-right: 10px;" />
            <span class="ml-2">{{ item.label }}</span>
            <span v-if="hasSubmenu" class="pi pi-fw pi-angle-down ml-2" />
          </a>

        </template>
      </Menubar>

    </div>
  </nav>
  <main>
    <RouterView />
  </main>
</template>