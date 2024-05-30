import { createApp } from "vue";
import App from "./App.vue";
import { createMemoryHistory, createRouter } from 'vue-router'
import HomeView from './views/HomeView.vue'
import AlertView from "@/views/AlertView.vue";

const routes = [
  { path: '/', component: HomeView },
  { path: '/alerts', component: AlertView }
]

const router = createRouter({
  history: createMemoryHistory(),
  routes,
})

createApp(App).use(router).mount("#app");
