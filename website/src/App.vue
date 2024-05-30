<script>
export default {
  name: "app"
};
</script>

<template>
  <nav>
    <RouterLink to="/">Home</RouterLink>
    <RouterLink to="/alerts">Alerts</RouterLink>
  </nav>
  <main>
    <RouterView />
  </main>
</template>