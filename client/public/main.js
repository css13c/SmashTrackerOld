const { app, BrowserWindow } = require("electron");
const path = require("path");
const isDev = require("electron-is-dev");

function createWindow() {
  // Create the browser window.
  win = new BrowserWindow({ width: 800, height: 600, menuBarVisible: false });
  // and load the index.html of the app.
  const startUrl = isDev
    ? "http://localhost:3000"
    : `file://${path.join(__dirname, "../build/index.html")}`;
  win.loadURL(startUrl);
  //win.setMenu(null);
  win.once("ready-to-show", () => win.show());
  win.on("closed", () => {
    win = null;
  });
}

app.on("ready", createWindow);
