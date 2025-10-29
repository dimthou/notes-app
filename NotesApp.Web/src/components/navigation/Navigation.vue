<script setup lang="ts">
import { useAuthStore } from '@/store/auth';
import NavLink from './NavLink.vue';
import { computed } from 'vue';

const auth = useAuthStore();

const greeting = computed(() => auth.user ? `Hello, ${auth.user.username}` : '');

const navigationLinks = {
  unauthenticated: [
    {
      to: '/login',
      label: 'Sign In',
      icon: 'M11 16l-4-4m0 0l4-4m-4 4h14m-5 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h7a3 3 0 013 3v1'
    },
    {
      to: '/register',
      label: 'Sign Up',
      icon: 'M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z'
    }
  ],
  authenticated: [
    {
      to: '/notes',
      label: 'My Notes',
      icon: 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z'
    }
  ]
};
</script>

<template>
  <nav class="mt-8 flex-1 px-2 space-y-2">
    <template v-if="!auth.token">
      <NavLink
        v-for="link in navigationLinks.unauthenticated"
        :key="link.to"
        v-bind="link"
      />
    </template>
    <template v-else>
      <!-- Add greeting -->
      <div class="px-2 py-2 mb-4 text-sm font-medium text-gray-600">
        {{ greeting }}
      </div>
      <NavLink
        v-for="link in navigationLinks.authenticated"
        :key="link.to"
        v-bind="link"
      />
      <button
        @click="auth.logout"
        class="w-full group flex items-center px-2 py-2 text-sm font-medium rounded-md text-gray-600 hover:bg-gray-50"
      >
        <svg
          class="mr-3 h-6 w-6 text-gray-400"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"
          />
        </svg>
        Sign Out
      </button>
    </template>
  </nav>
</template>
