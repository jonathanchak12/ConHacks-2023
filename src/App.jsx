import "./App.css";
import Navbar from "./Navbar";
import flashyConLogo from "./assets/FlashyCon Logo.png";
import About from "./pages/About.jsx";
import Courses from "./pages/Courses.jsx";
import Flashcards from "./pages/Flashcards.jsx";
import Home from "./pages/Home.jsx";
import Unit from "./pages/Unit.jsx";

function App() {
  let Component;
  switch (window.location.pathname) {
    case "/":
      Component = Home;
      break;
    case "/flashcards":
      Component = Flashcards;
      break;
    case "/courses":
      Component = Courses;
      break;
    case "/unit":
      Component = Unit;
      break;
    case "/about":
      Component = About;
      break;
  }

  return (
    <>
      <Navbar></Navbar>
      <div>
        <a href="/" target="_blank">
          <img className="logo" src={flashyConLogo} alt="FlashyCon Logo" />
        </a>
      </div>
      <Component></Component>
    </>
  );
}

export default App;
