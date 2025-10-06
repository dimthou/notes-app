<script setup lang="ts">
import { ref } from 'vue';
import Navigation from './Navigation.vue';

const sidebarOpen = ref(false);

function closeSidebar() {
  sidebarOpen.value = false;
}
</script>

<template>
  <div class="md:hidden">
    <div class="fixed inset-0 flex z-40" v-show="sidebarOpen">
      <div
        class="fixed inset-0 bg-gray-600 bg-opacity-75"
        @click="closeSidebar"
      />
      <div
        id="sidebar"
        class="relative flex-1 flex flex-col w-72 max-w-sm bg-white"
      >
        <div class="absolute top-0 right-0 -mr-12 pt-2">
          <button
            @click="closeSidebar"
            class="ml-1 flex items-center justify-center h-10 w-10 rounded-full focus:outline-none focus:ring-2 focus:ring-inset focus:ring-white"
          >
            <span class="sr-only">Close sidebar</span>
            <svg
              class="h-6 w-6 text-white"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </button>
        </div>
        <div class="flex-1 flex flex-col pt-5 pb-4 overflow-y-auto">
          <div class="flex items-center flex-shrink-0 px-4">
            <router-link
              to="/notes"
              class="flex items-center space-x-2"
              @click="closeSidebar"
            >
              <svg
                class="h-8 w-8 text-blue-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
                />
              </svg>
              <span class="text-xl font-semibold text-gray-900">NotesApp</span>
            </router-link>
          </div>
          <Navigation />
        </div>
      </div>
    </div>
  </div>
</template>
