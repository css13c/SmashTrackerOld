import React from "react";
import { createStyles, makeStyles } from "@material-ui/core/styles";

const useStyles = makeStyles((theme) =>
  createStyles({
    base: {
      border: "1px solid black",
      display: "flex",
      flexDirection: "column",
      padding: 5,
      margin: 5,
    },
    labelWrapper: {
      display: "flex",
      justifyContent: "space-between",
    },
    name: {},
    characters: {
      marginTop: 0,
      marginBottom: 0,
    },
  })
);

export const PlayerCard = ({ data }: Props) => {
  const classes = useStyles();

  return (
    <div className={classes.base}>
      <div className={classes.labelWrapper}>
        <div className={classes.name}>
          Name: {data.firstName} {data.lastName}
        </div>
        <div>Email: {data.email}</div>
      </div>
      <div>
        <div>Characters:</div>
        <ul className={classes.characters}>
          {data.characters &&
            data.characters.map((x) => {
              return <li key={`${data.id}-${x.id}`}>{x.name}</li>;
            })}
        </ul>
      </div>
    </div>
  );
};

interface Props {
  data: Player;
}
