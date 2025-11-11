// Give the service worker access to Firebase Messaging.
// Note that you can only use Firebase Messaging here. Other Firebase libraries
// are not available in the service worker.
importScripts('https://www.gstatic.com/firebasejs/10.7.1/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/10.7.1/firebase-messaging-compat.js');

// Initialize the Firebase app in the service worker by passing in
// your app's Firebase config object.
// https://firebase.google.com/docs/web/setup#config-object
firebase.initializeApp({
	apiKey: "AIzaSyAWJ6UMn6hqmcsLRJ3Q_F8Wt6s7lCzHsZ0",
	authDomain: "herodev-2bcd1.firebaseapp.com",
	projectId: "herodev-2bcd1",
	storageBucket: "herodev-2bcd1.firebasestorage.app",
	messagingSenderId: "871851152675",
	appId: "1:871851152675:web:9c2231cdb4c9d57b77a896",
});

// Retrieve an instance of Firebase Messaging so that it can handle background messages.
const messaging = firebase.messaging();

messaging.onBackgroundMessage((payload) =>
{
  console.log('> Background message received. ', payload);
  //SendMessage(jManagerName, jMessageReceived, "Background Message Received :\n# " + payload.notification.title + "\n# " + payload.notification.body);
  
  // Customize notification here
  //const notificationOptions = { body: payload.notification.body };
  //self.registration.showNotification(payload.notification.title, notificationOptions);
});