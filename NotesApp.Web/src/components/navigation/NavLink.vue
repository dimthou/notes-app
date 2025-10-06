<script setup lang="ts">
interface Props {
  to: string;
  icon: string;
  label: string;
  onClick?: () => void;
}

const props = defineProps<Props>();
</script>

<template>
  <router-link
    :to="to"
    class="group flex items-center px-2 py-2 text-sm font-medium rounded-md"
    :class="[$route.path === to ? 'bg-blue-50 text-blue-600' : 'text-gray-600 hover:bg-gray-50']"
    @click="onClick"
  >
    <svg
      class="mr-3 h-6 w-6"
      :class="[$route.path === to ? 'text-blue-600' : 'text-gray-400']"
      fill="none"
      viewBox="0 0 24 24"
      stroke="currentColor"
    >
      <path
        stroke-linecap="round"
        stroke-linejoin="round"
        stroke-width="2"
        :d="icon"
      />
    </svg>
    {{ label }}
  </router-link>
</template>
