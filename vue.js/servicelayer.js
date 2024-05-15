import axios from 'axios'
import authHeader from '@/components/utilities/auth-header'

export const keywordsService = () => {
  const API_URL = 'https://localhost:7156/api/portal'

  const getKeywords = () => {
    let headers = authHeader()
    return axios.get(API_URL + '/keywords', { headers: headers })
  }

  const addKeyword = (keyword) => {
    const dataObj = {
      keyword: keyword
    }

    let headers = authHeader()
    const url = `${API_URL}/keywords`
    return new Promise((resolve, reject) => {
      axios
        .post(url, dataObj, { headers: headers })
        .then((res) => {
          resolve(res.data)
        })
        .catch((error) => {
          reject(error)
        })
    })
  }

  const removeKeyword = (keyword) => {
    const dataObj = {
      keyword: keyword
    }

    let headers = authHeader()
    const deleteurl = `${API_URL}/deletekeyword`
    console.log(headers)
    return new Promise((resolve, reject) => {
      axios
        .post(deleteurl, dataObj, { headers: headers })
        .then((res) => {
          resolve(res.data)
        })
        .catch((error) => {
          reject(error)
        })
    })
  }

  return { getKeywords, addKeyword, removeKeyword }
}
