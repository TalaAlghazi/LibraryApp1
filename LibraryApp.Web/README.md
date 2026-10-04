# LibraryApp.Web

React front end for LibraryApp1, built with [Vite](https://vite.dev). It talks to the
`LibraryApp1.API` project; it has no database access of its own.

## Pages

- **Sign in / Create account**
- **Books**: browse and search, reserve a book; admins can add and delete books
- **My Account**: current bookings (with return requests), booking history, profile and password
- **All Bookings** (admins only): every book that is out; confirm returns

## Running locally

1. Start the API: in Visual Studio, set `LibraryApp1.API` as the startup project and run it
   with the **https** profile (it listens on `https://localhost:7259`).
2. Start the front end:

   ```bash
   cd LibraryApp.Web
   npm install      # first time only
   npm run dev
   ```

3. Open <http://localhost:5173>.

During development, Vite forwards every request that starts with `/api` to the API
(see `vite.config.js`). The browser therefore sees a single origin, so the sign-in cookie
works without any CORS setup. If the API runs on a different port, change `target` there.

## Scripts

| Command           | What it does                              |
| ----------------- | ----------------------------------------- |
| `npm run dev`     | Start the development server              |
| `npm run build`   | Build a production version into `dist/`   |
| `npm run preview` | Serve the production build locally        |
| `npm run lint`    | Check the code with ESLint                |

## Project structure

```
src/
  api.js              fetch helper for the API (JSON in/out, error messages)
  bookingHelpers.js   date formatting and booking status labels
  App.jsx             signed-in state, page switching, layout
  Header.jsx          top bar, navigation and account menu
  Login.jsx           sign-in page
  Register.jsx        registration page
  Books.jsx           books page (search, reserve, add/delete)
  MyBookings.jsx      current bookings and booking history
  AllBookings.jsx     admin view of all bookings
  Profile.jsx         profile details and password change
  ConfirmDialog.jsx   confirmation popup used before actions
  AuthArt.jsx         illustration beside the sign-in forms
  AuthField.jsx       input with icon and show/hide password
  HeroArt.jsx         illustration on the books page
  Icons.jsx           small SVG icons
  index.css           all styles
```