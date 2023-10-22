import "./App.css";
import Flashcard from "./Flashcard";
import reactLogo from "./assets/react.svg";
import viteLogo from "/vite.svg";

function App() {
  return (
    <>
      <div>
        <a href="https://vitejs.dev" target="_blank" rel="noreferrer">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank" rel="noreferrer">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Flashcards - ConHacks 2023</h1>

      <div>
        <Flashcard
          term="Group 7 - Byte Brawlers"
          definition="Dario Simpson, Jonathan Chak, Nav, Siyang He"
        />
      </div>
    </>
  );
}

export default App;
