<script setup lang="ts">
import { ref, onMounted, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useNotesStore } from "@/store/notes";

const notesStore = useNotesStore();
const route = useRoute();
const router = useRouter();
console.log('Route path:', route.path);  // Debug full path
console.log('Route params:', route.params);  // Debug route parameters
const isAddMode = route.path.endsWith('/new');  // Check full path instead
console.log('Is Add Mode:', isAddMode);  // Debug mode
const id = isAddMode ? null : Number(route.params.id);
console.log('Note ID:', id);  // Debug ID conversion
const title = ref("");
const content = ref("");

async function loadNote() {
  if (!isAddMode && id) {
    await notesStore.fetchNoteDetail(id);
    if (notesStore.selectedNote) {
      title.value = notesStore.selectedNote.title;
      content.value = notesStore.selectedNote.content || "";
    }
  } else {
    title.value = "";
    content.value = "";
    notesStore.clearSelectedNote();
  }
}

onMounted(loadNote);
watch(() => route.params.id, loadNote);

async function save() {
  if (!title.value.trim()) {
    alert("Title is required");
    return;
  }
  
  try {
    if (isAddMode) {
      await notesStore.createNote(title.value.trim(), content.value);
    } else if (id) {
      await notesStore.updateNote({
        id,
        title: title.value.trim(),
        content: content.value
      });
    }
    router.push("/notes");
  } catch (error: any) {
    if (error.response?.data?.error) {
      alert(error.response.data.error);
    } else {
      alert("An error occurred while saving the note");
    }
  }
}

async function handleDelete() {
  if (!id) return;
  if (confirm("Are you sure you want to delete this note?")) {
    await notesStore.deleteNote(id);
    router.push("/notes");
  }
}
</script>

<template>
  <div class="max-w-xl mx-auto bg-white p-6 rounded shadow mt-8">
    <h2 class="text-xl font-bold mb-4">{{ isAddMode ? 'Add Note' : 'Edit Note' }}</h2>
    <div class="mb-4">
      <label class="block text-sm font-medium text-gray-700 mb-1">Title</label>
      <input v-model="title" type="text" class="w-full border rounded px-3 py-2 focus:ring focus:ring-blue-300" />
    </div>
    <div class="mb-4">
      <label class="block text-sm font-medium text-gray-700 mb-1">Content</label>
      <textarea v-model="content" rows="4" class="w-full border rounded px-3 py-2 focus:ring focus:ring-blue-300"></textarea>
    </div>
    <div class="mb-4">
      <label class="block text-sm font-medium text-gray-700 mb-1">Created At</label>
      <input type="text" :value="notesStore.selectedNote ? new Date(notesStore.selectedNote.createdAt).toLocaleString() : ''" class="w-full border rounded px-3 py-2 bg-gray-100" readonly />
    </div>
    <div class="mb-4">
      <label class="block text-sm font-medium text-gray-700 mb-1">Updated At</label>
      <input type="text" :value="notesStore.selectedNote ? new Date(notesStore.selectedNote.updatedAt).toLocaleString() : ''" class="w-full border rounded px-3 py-2 bg-gray-100" readonly />
    </div>
    <div class="flex justify-end space-x-2">
      <button @click="() => router.push('/notes')" class="px-4 py-2 bg-gray-300 rounded hover:bg-gray-400">Cancel</button>
      <button @click="save" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">Save</button>
      <button v-if="!isAddMode" @click="handleDelete" class="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 ml-2">Delete</button>
    </div>
  </div>
</template>
