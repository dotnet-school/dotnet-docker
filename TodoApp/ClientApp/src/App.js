import React, { Component } from 'react';
import TodoApp from './components/TodoApp';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return <TodoApp baseUrl={'/api'}/>;
  }
}
