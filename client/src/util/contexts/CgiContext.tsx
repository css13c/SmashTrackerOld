import { Connection } from "electron-cgi";
import { createContext } from "react";

export interface CgiInterface {
  connection: Connection;
}

const context = createContext<Connection | null>(null);
export const CgiProvider = context.Provider;
export const CgiConsumer = context.Consumer;
