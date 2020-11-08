import { PlayerDb } from "components/PlayerDb";
import React from "react";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";

const App = () => {
  return (
    <Router>
      <Switch>
        <Route path="/">
          <PlayerDb />
        </Route>
      </Switch>
    </Router>
  );
};

export default App;
