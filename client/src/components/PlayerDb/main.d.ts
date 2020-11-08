interface Player {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  characters: Array<PlayerCharacter>;
}

interface PlayerCharacter {
  id: string;
  name: string;
}