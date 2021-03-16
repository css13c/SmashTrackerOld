import { makeStyles } from "@material-ui/core";
import React, { useEffect, useState } from "react";
import { PlayerCard } from "./components";

export const PlayerDb = (props: Props) => {
  const [playerData, setPlayerData] = useState<Array<Player> | null>(null);

  useEffect(() => {
    console.log("test2");
    setPlayerData([
      {
        id: "1",
        firstName: "Connor",
        lastName: "Smith",
        email: "connor@s.com",
        characters: [
          {
            id: "1",
            name: "Fox",
          },
        ],
      },
      {
        id: "2",
        firstName: "Avery",
        lastName: "Moran",
        email: "avery@s.com",
        characters: [
          {
            id: "1",
            name: "Fox",
          },
          {
            id: "2",
            name: "Falco",
          },
        ],
      },
    ]);
  }, []);

  return (
    <div>
      PlayerDb
      {playerData &&
        playerData.map((x) => {
          console.log("Test");
          return <PlayerCard key={x.id} data={x} />;
        })}
    </div>
  );
};

interface Props {}
