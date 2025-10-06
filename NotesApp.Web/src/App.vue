<script setup lang="ts">
import { ref, provide } from 'vue';
import Navigation from './components/navigation/Navigation.vue';
import MobileSidebar from './components/navigation/MobileSidebar.vue';

const sidebarOpen = ref(false);

// Provide sidebarOpen state to child components
provide('sidebarOpen', sidebarOpen);

function toggleSidebar() {
  sidebarOpen.value = !sidebarOpen.value;
}
</script>

<template>
  <div class="min-h-screen flex bg-gray-100">
    <!-- Desktop Sidebar -->
    <div class="hidden md:flex md:w-64 md:flex-col md:fixed md:inset-y-0">
      <div class="flex-1 flex flex-col min-h-0 bg-white shadow-lg">
        <div class="flex-1 flex flex-col pt-5 pb-4 overflow-y-auto">
          <div class="flex items-center flex-shrink-0 px-4">
            <router-link to="/notes" class="flex items-center space-x-2">
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

    <!-- Mobile Sidebar -->
    <MobileSidebar />

    <!-- Mobile Top Bar -->
    <div class="md:hidden fixed top-0 left-0 right-0 z-30 bg-white shadow-sm">
      <div class="px-4 py-3 flex items-center justify-between">
        <button
          id="sidebar-button"
          @click="toggleSidebar"
          class="inline-flex items-center justify-center p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100"
        >
          <span class="sr-only">Open sidebar</span>
          <svg
            class="h-6 w-6"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M4 6h16M4 12h16M4 18h16"
            />
          </svg>
        </button>
        <router-link to="/notes" class="flex items-center space-x-2">
          <span class="text-xl font-semibold text-gray-900">NotesApp</span>
        </router-link>
        <div class="w-6" />
      </div>
    </div>

    <!-- Main Content -->
    <div class="md:pl-64 flex flex-col flex-1">
      <main class="flex-1 py-6 px-4 sm:px-6 md:px-8">
        <div class="md:mt-0 mt-16">
          <router-view />
        </div>
      </main>
    </div>
  </div>
</template>

<style>
/* Hide scrollbar for Chrome, Safari and Opera */
.overflow-y-auto::-webkit-scrollbar {
  display: none;
}

/* Hide scrollbar for IE, Edge and Firefox */
.overflow-y-auto {
  -ms-overflow-style: none; /* IE and Edge */
  scrollbar-width: none; /* Firefox */
}
</style>
